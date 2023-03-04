using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.ProductMovings;
using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductMovingEventHandler : INotificationHandler<ResaveErpProductMovingEvent>
    {
        private readonly IProductMovingMapRepository _productMovingMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductMovingSaver _erpProductMovingSaver;

        public ResaveErpProductMovingEventHandler(
            IProductMovingMapRepository productMovingMapRepository,
            IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpProductMovingSaver erpProductMovingSaver)
        {
            _productMovingMapRepository = productMovingMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductMovingSaver = erpProductMovingSaver;
        }
        public async Task Handle(ResaveErpProductMovingEvent notification, CancellationToken cancellationToken)
        {
            foreach (var productMoving in notification.Messages)
            {
                var productMovingMapping = await _productMovingMapRepository.GetByErpAsync(productMoving.Id);
                _erpProductMovingSaver.InitErpObject(productMoving, productMovingMapping);

                var mappingInfo = await _erpProductMovingSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (productMovingMapping == null)
                {
                    await _erpProductMovingSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpProductMovingSaver.Update();
                }
                await _erpNotFullMappedRepository.RemoveAsync(productMoving.Id, MappingTypes.ProductMoving);
            }
        }
    }
}
