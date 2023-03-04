using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetProductsWithoutMapping
{
    public class GetProductsWithoutMappingQueryHandler : IRequestHandler<GetProductsWithoutMappingQuery, IEnumerable<Guid>>
    {
        private readonly LegacyDbConnection _db;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;

        public GetProductsWithoutMappingQueryHandler(
            IProductMapRepository productMapRepository, 
            LegacyDbConnection db, 
            IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _productMapRepository = productMapRepository;
            _db = db;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task<IEnumerable<Guid>> Handle(GetProductsWithoutMappingQuery request, CancellationToken cancellationToken)
        {
            var mappingsDict = (await _productMapRepository.GetRangeByErpIdsAsync(request.Data)).ToDictionary(e => e.ExternalMapId,e => e.LegacyId);
            var erpNotFullMappings = (await _erpNotFullMappedRepository.GetAllByTypeAsync(MappingTypes.Product)).ToDictionary(e => e.ErpId);

            var result = new List<Guid>();
            foreach (var erpGuid in request.Data)
            {
                if (!mappingsDict.ContainsKey(erpGuid) && !erpNotFullMappings.ContainsKey(erpGuid))
                {
                    result.Add(erpGuid);
                    continue;
                }

                if (mappingsDict.ContainsKey(erpGuid))
                {
                    var selectSqlQuery = @"select [КодТовара] from [dbo].[lSQL_v_Товары] 
                                         where [КодТовара]=@ProductId";
                    var sqlProductId = await _db.Connection.QueryFirstOrDefaultAsync<int>(selectSqlQuery, new
                    {
                        ProductId = mappingsDict[erpGuid]
                    });
                    if (sqlProductId == 0)
                    {
                        result.Add(erpGuid);
                    }
                }
            }
            return result;
        }
    }
}
