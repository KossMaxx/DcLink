using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypeCategories;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryMap
{
    public class AddProductTypeCategoryMapCommandHandler : IRequestHandler<AddProductTypeCategoryMapCommand>
    {
        private readonly IProductTypeCategoryMapRepository _productTypeCategoryMapRepository;

        public AddProductTypeCategoryMapCommandHandler(IProductTypeCategoryMapRepository productTypeCategoryMapRepository)
        {
            _productTypeCategoryMapRepository = productTypeCategoryMapRepository;
        }

        public async Task<Unit> Handle(AddProductTypeCategoryMapCommand command, CancellationToken cancellationToken)
        {
            var productTypeCategoryMap = await _productTypeCategoryMapRepository.GetByMapAsync(command.MessageId);
            if (productTypeCategoryMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productTypeCategoryMap.MapToExternalId(command.ExternalMapId);
            await _productTypeCategoryMapRepository.SaveAsync(productTypeCategoryMap, productTypeCategoryMap.Id);

            return new Unit();
        }
    }
}
