using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Diagnostics.GetTemporaryMappingCounts
{
    public class GetTemporaryMappingCountsQueryHandler : IRequestHandler<GetTemporaryMappingCountsQuery, IEnumerable<NotFullMappingCountsDto>>
    {
        private readonly AppDbContext _appDb;
        private readonly AppDbConnection _appDbConnection;

        public GetTemporaryMappingCountsQueryHandler(AppDbContext appDb, AppDbConnection appDbConnection)
        {
            _appDb = appDb;
            _appDbConnection = appDbConnection;
        }

        public async Task<IEnumerable<NotFullMappingCountsDto>> Handle(GetTemporaryMappingCountsQuery request, CancellationToken cancellationToken)
        {
            var notFullMappingsCounts = new List<NotFullMappingCountsDto>();
            var contextTypes = _appDb.Model.GetEntityTypes().Select(e=>e.ClrType).ToList();
            foreach (var type in contextTypes)
            {
                if (type.BaseType == typeof(BaseMapModel) || type == typeof(ClassMapEF) || type == typeof(ManufacturerMapEF))
                {
                    var tableName = type.Name.Replace("EF", "s");
                    var sqlQuery = $@"SELECT count(*) FROM public.""{tableName}"" where ""ErpGuid"" is null";
                    var result = (await _appDbConnection.Connection.QueryAsync<int>(sqlQuery)).FirstOrDefault();
                    notFullMappingsCounts.Add(new NotFullMappingCountsDto
                    {
                        EntityType = tableName,
                        Quantity = result
                    });
                }
            }

            return notFullMappingsCounts;
        }
    }
}
