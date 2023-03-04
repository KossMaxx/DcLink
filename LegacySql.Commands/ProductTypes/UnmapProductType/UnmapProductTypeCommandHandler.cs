using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypes;
using MediatR;

namespace LegacySql.Commands.ProductTypes.UnmapProductType
{
    public class UnmapProductTypeCommandHandler : IRequestHandler<UnmapProductTypeCommand>
    {
        private readonly IProductTypeMapRepository _productTypeMapRepository;

        public UnmapProductTypeCommandHandler(IProductTypeMapRepository productTypeMapRepository)
        {
            _productTypeMapRepository = productTypeMapRepository;
        }

        public async Task<Unit> Handle(UnmapProductTypeCommand request, CancellationToken cancellationToken)
        {
            await _productTypeMapRepository.RemoveByErpAsync(request.ErpId);
            return new Unit();
        }
    }
}
