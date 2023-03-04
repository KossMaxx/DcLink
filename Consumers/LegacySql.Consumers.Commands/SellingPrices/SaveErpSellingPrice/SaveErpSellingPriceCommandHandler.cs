using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.SellingPrices.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.SellingPrices.SaveErpSellingPrice
{
    public class SaveErpSellingPriceCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpSellingPriceDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IProductMapRepository _productMapRepository;
        private ErpSellingPriceSaver _erpSellingPriceSaver;

        public SaveErpSellingPriceCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpSellingPriceSaver erpSellingPriceSaver, 
            IProductMapRepository productMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpSellingPriceSaver = erpSellingPriceSaver;
            _productMapRepository = productMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpSellingPriceDto> command, CancellationToken cancellationToken)
        {
            var price = command.Value;
            var productMapping = await _productMapRepository.GetByErpAsync(price.ProductId);
            _erpSellingPriceSaver.InitErpObject(price,productMapping);

            var isMappingFull = _erpSellingPriceSaver.GetMappingInfo(price);
            if (!isMappingFull.IsMappingFull)
            {
                await SaveNotFullMapping(price, isMappingFull.Why);
                return new Unit();
            }

            await _erpSellingPriceSaver.Update();
            await _erpNotFullMappedRepository.RemoveAsync(price.ProductId, MappingTypes.SellingPrice);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpSellingPriceDto price, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                price.ProductId,
                MappingTypes.SellingPrice,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(price)
            ));
        }
    }
}