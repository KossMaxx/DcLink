using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.MovementOrders;
using LegacySql.Domain.MovementOrders;
using LegacySql.Domain.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpMovementOrderEventHandler : INotificationHandler<ResaveErpMovementOrderEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IMovementOrderMapRepository _movementOrderMapRepository;
        private ErpMovementOrderSaver _erpMovementOrderSaver;

        public ResaveErpMovementOrderEventHandler(IErpNotFullMappedRepository erpNotFullMappedRepository,
           IMovementOrderMapRepository movementOrderMapRepository,
           ErpMovementOrderSaver erpMovementOrderSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _movementOrderMapRepository = movementOrderMapRepository;
            _erpMovementOrderSaver = erpMovementOrderSaver;
        }

        public async Task Handle(ResaveErpMovementOrderEvent notification, CancellationToken cancellationToken)
        {
            foreach (var order in notification.Messages)
            {
                var orderMapping = await _movementOrderMapRepository.GetByErpAsync(order.Id);
                _erpMovementOrderSaver.InitErpObject(order, orderMapping);

                var mappingInfo = await _erpMovementOrderSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpMovementOrderSaver.Save(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(order.Id, MappingTypes.MovementOrder);
            }
        }
    }
}
