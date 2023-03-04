using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ClientOrder.Export;
using MessageBus.ClientOrder.Export.Add;
using MessageBus.ClientOrder.Export.Change;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.ClientOrders.PublishClientOrderArchival
{
    public class PublishClientOrderArchivalCommandHandler : ManagedCommandHandler<PublishClientOrderArchivalCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyClientOrderRepository _clientOrderRepository;
        private readonly ILegacyClientOrderArchivalRepository _clientOrderArchivalRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ClientOrderMapper _mapper;

        public PublishClientOrderArchivalCommandHandler(
            IBus bus, 
            ILegacyClientOrderRepository clientOrderRepository, 
            IClientOrderMapRepository clientOrderMapRepository, 
            ILegacyClientOrderArchivalRepository clientOrderArchivalRepository, 
            ILogger<PublishClientOrderArchivalCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _bus = bus;
            _clientOrderRepository = clientOrderRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _clientOrderArchivalRepository = clientOrderArchivalRepository;
            _messageFactory = messageFactory;
            _mapper = new ClientOrderMapper();
        }

        public override async Task HandleCommand(PublishClientOrderArchivalCommand command, CancellationToken cancellationToken)
        {
            var orders = (await _clientOrderRepository.GetClientOrdersWithNotEndedWarrantyAsync(cancellationToken)).ToList();
            orders.AddRange(await _clientOrderArchivalRepository.GetClientOrdersAsync(cancellationToken));
            foreach (var clientOrder in orders)
            {
                var mappingInfo = clientOrder.IsMappingsFull();
                if (mappingInfo.IsMappingFull)
                {
                    var clientOrderDto = _mapper.MapToDto(clientOrder);
                    if (clientOrder.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyClientOrderMessage, ClientOrderDto>(clientOrder.Id.ExternalId.Value, clientOrderDto);
                        await _bus.Publish(message, cancellationToken);
                    }

                    if (clientOrder.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddClientOrderMessage, ClientOrderDto>(clientOrderDto);
                        await _bus.Publish(message, cancellationToken);

                        var mapping = new ExternalMap(message.MessageId, clientOrder.Id.InnerId);
                        await _clientOrderMapRepository.SaveAsync(mapping);
                    }
                }
            }
        }
    }
}
