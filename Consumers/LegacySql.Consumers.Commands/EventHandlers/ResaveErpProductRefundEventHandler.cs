using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.ProductRefunds;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductRefundEventHandler : INotificationHandler<ResaveErpProductRefundEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IProductRefundMapRepository _productRefundMapRepository;
        private ErpProductRefundSaver _erpProductRefundSaver;

        public ResaveErpProductRefundEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IProductRefundMapRepository productRefundMapRepository, 
            ErpProductRefundSaver erpProductRefundSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _productRefundMapRepository = productRefundMapRepository;
            _erpProductRefundSaver = erpProductRefundSaver;
        }

        public async Task Handle(ResaveErpProductRefundEvent notification, CancellationToken cancellationToken)
        {
            foreach (var entity in notification.Messages)
            {
                var productMapping = await _productRefundMapRepository.GetByErpAsync(entity.Id);

                _erpProductRefundSaver.InitErpObject(entity, productMapping);

                var mapInfo = await _erpProductRefundSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpProductRefundSaver.SaveErpObject(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(entity.Id, MappingTypes.ProductRefund);
            }
        }
    }
}
