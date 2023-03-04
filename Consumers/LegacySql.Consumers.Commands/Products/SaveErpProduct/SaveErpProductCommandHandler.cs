using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Products.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Products.SaveErpProduct
{
    public class SaveErpProductCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpProductDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IProductMapRepository _productMapRepository;
        private ErpProductSaver _erpProductSaver;

        public SaveErpProductCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, IProductMapRepository productMapRepository, ErpProductSaver erpProductSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _productMapRepository = productMapRepository;
            _erpProductSaver = erpProductSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpProductDto> command, CancellationToken cancellationToken)
        {
            var product = command.Value;
            var productMapping = await _productMapRepository.GetByErpAsync(product.Id);

            if (productMapping == null && !product.TypeId.HasValue)
            {
                throw new ArgumentNullException($"При создании товара (id:{product.Id}), его тип не может быть пустым");
            }

            _erpProductSaver.InitErpObject(product, productMapping);
            
            var mapInfo = await _erpProductSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(product, mapInfo.Why);
                return new Unit();
            }

            await _erpProductSaver.SaveErpObject(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(product.Id, MappingTypes.Product);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpProductDto product, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                product.Id,
                MappingTypes.Product,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(product)
            ));
        }
    }
}