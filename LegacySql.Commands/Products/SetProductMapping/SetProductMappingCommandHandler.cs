using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;
using MediatR;

namespace LegacySql.Commands.Products.SetProductMapping
{
    public class SetProductMappingCommandHandler : IRequestHandler<SetProductMappingCommand>
    {
        private readonly IProductMapRepository _productMapRepository;

        public SetProductMappingCommandHandler(IProductMapRepository productMapRepository)
        {
            _productMapRepository = productMapRepository;
        }

        public async Task<Unit> Handle(SetProductMappingCommand command, CancellationToken cancellationToken)
        {
            var productMap = await _productMapRepository.GetByLegacyAsync(command.InnerId);

            if (productMap != null)
            {
                productMap.MapToExternalId(command.ExternalId);
                await _productMapRepository.SaveAsync(productMap, productMap.Id);
            }

            return new Unit();
        }
    }
}
