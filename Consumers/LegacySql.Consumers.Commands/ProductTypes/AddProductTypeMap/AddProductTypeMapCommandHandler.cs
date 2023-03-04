using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypes;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeMap
{
    public class AddProductTypeMapCommandHandler : IRequestHandler<AddProductTypeMapCommand>
    {
        private readonly IProductTypeMapRepository _productTypeMapRepository;

        public AddProductTypeMapCommandHandler(IProductTypeMapRepository productTypeMapRepository)
        {
            _productTypeMapRepository = productTypeMapRepository;
        }

        public async Task<Unit> Handle(AddProductTypeMapCommand command, CancellationToken cancellationToken)
        {
            var productTypeMap = await _productTypeMapRepository.GetByMapAsync(command.MessageId);
            if (productTypeMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productTypeMap.MapToExternalId(command.ExternalMapId);
            await _productTypeMapRepository.SaveAsync(productTypeMap, productTypeMap.Id);

            return new Unit();
        }
    }
}
