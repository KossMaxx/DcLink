using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ClientOrders;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpClientOrderEventHandler : INotificationHandler<ResaveErpClientOrderEvent>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpClientOrderSaver _erpClientOrderSaver;
        private IEnumerable<ExternalMap> _clientOrderMapping;

        public ResaveErpClientOrderEventHandler(
            IClientOrderMapRepository clientOrderMapRepository, 
            ErpClientOrderSaver erpClientOrderSaver, 
            IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _erpClientOrderSaver = erpClientOrderSaver;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task Handle(ResaveErpClientOrderEvent notification, CancellationToken cancellationToken)
        {
            foreach (var order in notification.Messages)
            {
                _clientOrderMapping = await _clientOrderMapRepository.GetRangeByErpAsync(order.Id);

                _erpClientOrderSaver.InitErpObject(order, _clientOrderMapping);

                var mapInfo = await _erpClientOrderSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpClientOrderSaver.SaveErpObject();
                await _erpNotFullMappedRepository.RemoveAsync(order.Id, MappingTypes.ClientOrder);
            }
        }
    }
}
