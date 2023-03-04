using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.SellingPrices;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpSellingPriceEventHandler : INotificationHandler<ResaveErpSellingPriceEvent>
    {
        private readonly IProductMapRepository _productMapRepository;
        private ErpSellingPriceSaver _erpSellingPriceSaver;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;

        public ResaveErpSellingPriceEventHandler(IProductMapRepository productMapRepository, 
            ErpSellingPriceSaver erpSellingPriceSaver, IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _productMapRepository = productMapRepository;
            _erpSellingPriceSaver = erpSellingPriceSaver;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task Handle(ResaveErpSellingPriceEvent notification, CancellationToken cancellationToken)
        {
            foreach (var price in notification.Messages)
            {
                var productMapping = await _productMapRepository.GetByErpAsync(price.ProductId);
                _erpSellingPriceSaver.InitErpObject(price, productMapping);

                var isMappingFull = _erpSellingPriceSaver.GetMappingInfo(price);
                if (!isMappingFull.IsMappingFull)
                {
                    continue;
                }

                await _erpSellingPriceSaver.Update();
                await _erpNotFullMappedRepository.RemoveAsync(price.ProductId, MappingTypes.SellingPrice);
            }
        }
    }
}