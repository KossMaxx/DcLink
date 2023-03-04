using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Legacy.Data;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetActualProductsIds
{
    public class GetActualProductsIdsQueryHandler : IRequestHandler<GetActualProductsIdsQuery, IEnumerable<long>>
    {
        private readonly LegacyDbConnection _legacyDbConnection;

        public GetActualProductsIdsQueryHandler(LegacyDbConnection legacyDbConnection)
        {
            _legacyDbConnection = legacyDbConnection;
        }

        public async Task<IEnumerable<long>> Handle(GetActualProductsIdsQuery request, CancellationToken cancellationToken)
        {
            var selectSqlIdsQuery = @"select [КодТовара] from [dbo].[lSQL_v_Товары]";
            return await _legacyDbConnection.Connection.QueryAsync<long>(selectSqlIdsQuery);
        }
    }
}
