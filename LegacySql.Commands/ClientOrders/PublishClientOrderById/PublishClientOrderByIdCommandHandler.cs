using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Shared;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Export;
using MessageBus.ClientOrder.Export.Add;
using MessageBus.ClientOrder.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.ClientOrders.PublishClientOrderById
{
    public class PublishClientOrderByIdCommandHandler : IRequestHandler<PublishClientOrderByIdCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyClientOrderRepository _clientOrderRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly ILogger<PublishClientOrderByIdCommandHandler> _logger;
        private readonly ClientOrderMapper _mapper;

        public PublishClientOrderByIdCommandHandler(
            IBus bus,
            ILegacyClientOrderRepository clientOrderRepository,
            IClientOrderMapRepository clientOrderMapRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger, 
            ILogger<PublishClientOrderByIdCommandHandler> logger)
        {
            _bus = bus;
            _clientOrderRepository = clientOrderRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _logger = logger;
            _mapper = new ClientOrderMapper();
        }

        public async Task<Unit> Handle(PublishClientOrderByIdCommand command, CancellationToken cancellationToken)
        {
            var order = await _clientOrderRepository.GetClientOrderWithAllFiltersAsync(command.Id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException("Клиентский заказ не найден");
            }

            if (!order.Items.Any())
            {
                _logger.LogInformation($"LegacySql | client-orders | CommandHandler: {GetType().Name} Ignored. Don't have items");
                return new Unit();
            }
            
            var mappingInfo = order.IsMappingsFull();
            if (!mappingInfo.IsMappingFull)
            {
                _logger.LogInformation($"LegacySql | client-orders | CommandHandler: {GetType().Name} Ignored. Has notfull mapping ({mappingInfo.Why})");
                return new Unit();
            }

            if (order.IsChanged())
            {
                var clientOrderDto = _mapper.MapToDto(order);
                var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyClientOrderMessage, ClientOrderDto>(order.Id.ExternalId.Value, clientOrderDto);
                await _bus.Publish(message, cancellationToken);

                _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Number, JsonConvert.SerializeObject(clientOrderDto));
            }

            if (order.IsNew())
            {
                var orderDto = _mapper.MapToDto(order);
                var message = _messageFactory.CreateNewEntityMessage<AddClientOrderMessage, ClientOrderDto>(orderDto);
                await _bus.Publish(message, cancellationToken);

                _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Number, JsonConvert.SerializeObject(orderDto));

                var mapping = new ExternalMap(message.MessageId, order.Id.InnerId);
                await _clientOrderMapRepository.SaveAsync(mapping);
            }

            return new Unit();
        }
    }
}
