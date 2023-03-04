using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypeCategoryGroups;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductTypeCategoryGroups.AddProductTypeCategoryGroupMap
{
    public class AddProductTypeCategoryGroupMapCommandHandler : IRequestHandler<AddProductTypeCategoryGroupMapCommand>
    {
        private readonly IProductTypeCategoryGroupMapRepository _productTypeCategoryGroupMapRepository;

        public AddProductTypeCategoryGroupMapCommandHandler(IProductTypeCategoryGroupMapRepository productTypeCategoryGroupMapRepository)
        {
            _productTypeCategoryGroupMapRepository = productTypeCategoryGroupMapRepository;
        }

        public async Task<Unit> Handle(AddProductTypeCategoryGroupMapCommand command, CancellationToken cancellationToken)
        {
            var productTypeCategoryGroupMap = await _productTypeCategoryGroupMapRepository.GetByMapAsync(command.MessageId);
            if (productTypeCategoryGroupMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productTypeCategoryGroupMap.MapToExternalId(command.ExternalMapId);
            await _productTypeCategoryGroupMapRepository.SaveAsync(productTypeCategoryGroupMap, productTypeCategoryGroupMap.Id);

            return new Unit();
        }
    }
}
