using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductSubtypes;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductSubtypes.AddProductSubtypeMap
{
    public class AddProductSubtypeMapCommandHandler : IRequestHandler<AddProductSubtypeMapCommand>
    {
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;

        public AddProductSubtypeMapCommandHandler(IProductSubtypeMapRepository productSubtypeMapRepository)
        {
            _productSubtypeMapRepository = productSubtypeMapRepository;
        }

        public async Task<Unit> Handle(AddProductSubtypeMapCommand command, CancellationToken cancellationToken)
        {
            var productSubtypeMap = await _productSubtypeMapRepository.GetByMapAsync(command.MessageId);
            if (productSubtypeMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productSubtypeMap.MapToExternalId(command.ExternalMapId);
            await _productSubtypeMapRepository.SaveAsync(productSubtypeMap, productSubtypeMap.Id);

            return new Unit();
        }
    }
}
