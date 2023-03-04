using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Extensions;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ClientOrder.Export;
using MessageBus.ClientOrder.Export.Add;
using MessageBus.ClientOrder.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;

namespace LegacySql.Commands.ClientOrders.PublishClientOrder
{
    public class PublishClientOrderCommandHandler : ManagedCommandHandler<PublishClientOrderCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyClientOrderRepository _clientOrderRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly ClientOrderMapper _mapper;

        public PublishClientOrderCommandHandler(
            IBus bus,
            ILegacyClientOrderRepository clientOrderRepository,
            IClientOrderMapRepository clientOrderMapRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILogger<PublishClientOrderCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            IErpChangedRepository erpChangedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _bus = bus;
            _clientOrderRepository = clientOrderRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _erpChangedRepository = erpChangedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _mapper = new ClientOrderMapper();
        }

        public override async Task HandleCommand(PublishClientOrderCommand command, CancellationToken cancellationToken)
        {
            await Publish(command, cancellationToken);
        }

        public async Task Publish(PublishClientOrderCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.ClientOrder);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var erpChangedOrders = (await _erpChangedRepository.GetAll(typeof(ClientOrder).Name))
                .ToDictionary(e => e.LegacyId, e => e.Date);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ClientOrder));

            IAsyncEnumerable<ClientOrder> clientOrders;
            if (command.Id.HasValue)
            {
                clientOrders = GetClientOrderAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                clientOrders = _clientOrderRepository.GetChangedClientOrdersAsync(
                    lastChangedDate, notFullMappingIds, cancellationToken);
            }


            var lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            await foreach (var clientOrder in clientOrders)
            {
                if (!clientOrder.Items.Any())
                {
                    continue;
                }

                if (clientOrder.ChangedAt.HasValue)
                {
                    lastDate.Add(clientOrder.ChangedAt.Value);
                }

                if (erpChangedOrders.ContainsKey(clientOrder.Id.InnerId) &&
                    await IsCheckErpChanged(clientOrder.Id.InnerId, clientOrder.ChangedAt, erpChangedOrders))
                {
                    continue;
                }

                var mappingInfo = clientOrder.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    if (!notFullMappingsIdsDictionary.ContainsKey(clientOrder.Id.InnerId))
                    {
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(clientOrder.Id.InnerId,
                            MappingTypes.ClientOrder, DateTime.Now, mappingInfo.Why));
                    }

                    continue;
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (clientOrder.IsChanged())
                    {
                        var clientOrderDto = _mapper.MapToDto(clientOrder);
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyClientOrderMessage, ClientOrderDto>(clientOrder.Id.ExternalId.Value, clientOrderDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Number, JsonConvert.SerializeObject(clientOrderDto));
                    }

                    if (clientOrder.IsNew())
                    {
                        var orderDto = _mapper.MapToDto(clientOrder);
                        var message = _messageFactory.CreateNewEntityMessage<AddClientOrderMessage, ClientOrderDto>(orderDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Number, JsonConvert.SerializeObject(orderDto));

                        var mapping = new ExternalMap(message.MessageId, clientOrder.Id.InnerId);
                        await _clientOrderMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(clientOrder.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(clientOrder.Id.InnerId,
                            MappingTypes.ClientOrder));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(ClientOrder), lastDate.Max());
            }
        }

        private async IAsyncEnumerable<ClientOrder> GetClientOrderAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var order = await _clientOrderRepository.GetClientOrderAsync(id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException("Клиентский заказ не найден");
            }

            yield return order;
        }

        private async Task<bool> IsCheckErpChanged(int clientOrderId, DateTime? clientOrderChangedAt,
            Dictionary<int, DateTime> erpChangedOrders)
        {
            var type = GetType();
            var entityName = type.GetEntityName();

            if (erpChangedOrders[clientOrderId] == clientOrderChangedAt)
            {
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {clientOrderId} Ignored because changed by ERP");
                return true;
            }

            await _erpChangedRepository.Delete(clientOrderId, typeof(ClientOrder).Name);
            _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {clientOrderId} Delete from ErpChanged");

            return false;
        }
    }
}