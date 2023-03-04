using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypes;
using MediatR;

namespace LegacySql.Commands.ProductTypes.MapProductType
{    
    public class MapProductTypeCommandHandler : IRequestHandler<MapProductTypeCommand>
    {
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly ILegacyProductTypeRepository _legacyProductTypeRepository;

        public MapProductTypeCommandHandler(IProductTypeMapRepository productTypeMapRepository, ILegacyProductTypeRepository legacyProductTypeRepository)
        {
            _productTypeMapRepository = productTypeMapRepository;
            _legacyProductTypeRepository = legacyProductTypeRepository;
        }

        public async Task<Unit> Handle(MapProductTypeCommand command, CancellationToken cancellationToken)
        {
            var productMap = await _productTypeMapRepository.GetByLegacyAsync(command.InnerId);

            if (productMap != null)
            {
                productMap.MapToExternalId(command.ExternalId);
                await _productTypeMapRepository.SaveAsync(productMap, productMap.Id);
            }
            else
            {
                var legacyProductTypeEf = await _legacyProductTypeRepository.Get(command.InnerId, cancellationToken);

                if (legacyProductTypeEf == null)
                {
                    throw new KeyNotFoundException($"Тип продукта с Id: {command.InnerId} не найден");
                }

                productMap = new ProductTypeMap(Guid.NewGuid(), command.InnerId, command.ExternalId);
                await _productTypeMapRepository.SaveAsync(productMap);
            }
            
            return new Unit();

        }
    }
}
