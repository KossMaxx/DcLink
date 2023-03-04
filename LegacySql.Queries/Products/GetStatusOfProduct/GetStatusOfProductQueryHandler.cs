using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Products.GetStatusOfProduct
{
    public class GetStatusOfProductQueryHandler : IRequestHandler<GetStatusOfProductQuery, ProductStatusDto>
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _appDb;

        public GetStatusOfProductQueryHandler(LegacyDbContext db, AppDbContext appDb)
        {
            _db = db;
            _appDb = appDb;
        }

        public async Task<ProductStatusDto> Handle(GetStatusOfProductQuery request, CancellationToken cancellationToken)
        {
            var existInSql = await _db.Products.AnyAsync(p => p.Code == request.ProductId, cancellationToken: cancellationToken);
            var hasMappingError = await _appDb.NotFullMapped.AnyAsync(p => p.InnerId == request.ProductId && p.Type == MappingTypes.Product, cancellationToken);
            
            var productMapping = await _appDb.ProductMaps.FirstOrDefaultAsync(p => p.LegacyId == request.ProductId || p.ErpGuid == request.ErpGuid, cancellationToken);
            var mappingStatus = productMapping?.MappingStatus(request.ProductId) ?? MappingStatuses.None;

            return new ProductStatusDto
            {
                InnerId = request.ProductId,
                ErpGuid = request.ErpGuid,
                ExistInSql = existInSql,
                HasMappingErrors = hasMappingError,
                MappingStatus = mappingStatus.ToString()
            };
        }
    }
}
