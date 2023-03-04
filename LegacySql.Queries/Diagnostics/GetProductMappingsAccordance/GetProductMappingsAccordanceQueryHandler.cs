using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetProductMappingsAccordance
{
    public class GetProductMappingsAccordanceQueryHandler : IRequestHandler<GetProductMappingsAccordanceQuery, ProductMappingsAccordanceResponse>
    {
        private readonly AppDbConnection _appDbConnection;
        private readonly LegacyDbConnection _legacyDbConnection;

        public GetProductMappingsAccordanceQueryHandler(AppDbConnection appDbConnection, LegacyDbConnection legacyDbConnection)
        {
            _appDbConnection = appDbConnection;
            _legacyDbConnection = legacyDbConnection;
        }

        public async Task<ProductMappingsAccordanceResponse> Handle(GetProductMappingsAccordanceQuery request, CancellationToken cancellationToken)
        {
            var selectSqlIdsQuery = @"select [КодТовара] from [dbo].[lSQL_v_Товары]";
            var sqlIds = (await _legacyDbConnection.Connection.QueryAsync<long>(selectSqlIdsQuery)).ToList();

            var selectServiceIdsQuery = $@"select ""LegacyId"" from public.""ProductMaps""";
            var serviceIds = (await _appDbConnection.Connection.QueryAsync<long>(selectServiceIdsQuery)).ToList();

            var selectServiceTemporaryIdsQuery = $@"select ""LegacyId"" from public.""ProductMaps"" where ""ErpGuid"" is null";
            var serviceTemporaryIds = (await _appDbConnection.Connection.QueryAsync<long>(selectServiceTemporaryIdsQuery)).ToList();

            var selectProductNotFullMappingCountQuery = $@"select count(*) from public.""NotFullMapped"" where ""Type""=1";
            var notFullMappingCount = await _appDbConnection.Connection.QueryFirstAsync<int>(selectProductNotFullMappingCountQuery);

            var sqlIdsCount = sqlIds.Count;
            var serviceIdsCount = serviceIds.Count;

            var excessInService = new List<long>();
            var deficitInService = new List<long>();
            if (sqlIdsCount > serviceIdsCount)
            {
                deficitInService = sqlIds.Except(serviceIds).ToList();
            }

            if (sqlIdsCount < serviceIdsCount)
            {
                excessInService = serviceIds.Except(sqlIds).ToList();
            }

            return new ProductMappingsAccordanceResponse
            {
                SqlIdsCount = sqlIdsCount,
                MappingsCount = serviceIdsCount,
                TemporaryMappingCount = serviceTemporaryIds.Count,
                ExcessInService = excessInService.Count,
                DeficitInService = deficitInService.Count,
                InNotFullMapping = notFullMappingCount
            };
        }
    }
}
