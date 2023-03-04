using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ClientOrders;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpClientOrderDeliveryEventHandler : INotificationHandler<ResaveErpClientOrderDeliveryEvent>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientOrderDeliverySaver _clientOrderDeliverySaver;

        public ResaveErpClientOrderDeliveryEventHandler(
            IClientOrderMapRepository clientOrderMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientOrderDeliverySaver clientOrderDeliverySaver)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _clientOrderDeliverySaver = clientOrderDeliverySaver;
        }

        public async Task Handle(ResaveErpClientOrderDeliveryEvent notification, CancellationToken cancellationToken)
        {
            foreach (var delivery in notification.Messages)
            {
                var clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(delivery.OrderId);
                _clientOrderDeliverySaver.InitErpObject(delivery, clientOrderMapping);

                var mappingInfo = _clientOrderDeliverySaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _clientOrderDeliverySaver.Save();
                await _erpNotFullMappedRepository.RemoveAsync(delivery.OrderId, MappingTypes.ClientOrderDelivery);
            }
        }
    }
}
