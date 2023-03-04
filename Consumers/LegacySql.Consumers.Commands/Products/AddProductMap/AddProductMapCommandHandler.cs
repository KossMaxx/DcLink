using System.Collections.Generic;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;

namespace LegacySql.Consumers.Commands.Products.AddProductMap
{
    public class AddProductMapCommandHandler : IRequestHandler<AddProductMapCommand>
    {
        private readonly IProductMapRepository _productMapRepository;

        public AddProductMapCommandHandler(IProductMapRepository productMapRepository)
        {
            _productMapRepository = productMapRepository;
        }

        public async Task<Unit> Handle(AddProductMapCommand command, CancellationToken cancellationToken)
        {
            var productMap = await _productMapRepository.GetByMapAsync(command.MessageId);
            if (productMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productMap.MapToExternalId(command.ExternalMapId);
            await _productMapRepository.SaveAsync(productMap, productMap.Id);

            return new Unit();
        }
    }
}
