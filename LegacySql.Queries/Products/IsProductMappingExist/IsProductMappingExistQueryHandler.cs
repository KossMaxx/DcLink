using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;
using MediatR;

namespace LegacySql.Queries.Products.IsProductMappingExist
{
    public class IsProductMappingExistQueryHandler : IRequestHandler<IsProductMappingExistQuery, bool>
    {
        private readonly IProductMapRepository _productMapRepository;

        public IsProductMappingExistQueryHandler(IProductMapRepository productMapRepository)
        {
            _productMapRepository = productMapRepository;
        }

        public async Task<bool> Handle(IsProductMappingExistQuery request, CancellationToken cancellationToken)
        {
            return await _productMapRepository.IsMappingExist(request.Id);
        }
    }
}
