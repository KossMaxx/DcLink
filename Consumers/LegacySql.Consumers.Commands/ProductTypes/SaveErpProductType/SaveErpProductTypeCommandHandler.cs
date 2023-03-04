using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductTypes.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ProductTypes.SaveErpProductType
{
    public class SaveErpProductTypeCommandHandler : IRequestHandler<BaseSaveErpCommand<ProductTypeErpDto>>
    {
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductTypeSaver _erpProductTypeSaver;

        public SaveErpProductTypeCommandHandler(
            IProductTypeMapRepository productTypeMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpProductTypeSaver erpProductTypeSaver)
        {
            _productTypeMapRepository = productTypeMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductTypeSaver = erpProductTypeSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ProductTypeErpDto> command, CancellationToken cancellationToken)
        {
            var type = command.Value;
            var mappingInfo = await _erpProductTypeSaver.GetMappingInfo(type);
            //if (!mappingInfo.IsMappingFull)
            //{
            //    await SaveNotFullMapping(type, mappingInfo.Why);
            //    return new Unit();
            //}

            var productTypeMap = await _productTypeMapRepository.GetByErpAsync(type.Id);
            _erpProductTypeSaver.InitErpObject(type, productTypeMap);
            if (productTypeMap != null)
            {
                await _erpProductTypeSaver.Update(command.MessageId);
            }
            else
            {
                await _erpProductTypeSaver.Create(command.MessageId);
            }
            await _erpNotFullMappedRepository.RemoveAsync(type.Id, MappingTypes.ProductType);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ProductTypeErpDto type, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                type.Id,
                MappingTypes.ProductType,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(type)
            ));
        }
    }
}
