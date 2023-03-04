using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Products.GetNotActualGuids
{
    public class GetNotActualGuidsRequestHandler : IRequestHandler<GetNotActualGuidsRequest>
    {
        private readonly IDbConnection _db;
        private readonly AppDbContext _mapDb;

        public GetNotActualGuidsRequestHandler(IDbConnection db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<Unit> Handle(GetNotActualGuidsRequest request, CancellationToken cancellationToken)
        {
            var countQuery = "select count(*) from lSQL_v_Товары";
            var count = await _db.QueryFirstOrDefaultAsync<int>(countQuery, commandTimeout: 300);
            var portion = 10000;
            var cycleLimitation = (double) count / portion;
            var actualIds = new List<long>();
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                var sqlQuery =
                    $"select [КодТовара] from lSQL_v_Товары order by [КодТовара] desc OFFSET {portion * i}  rows fetch next {portion} rows only";
                var tempIds = (await _db.QueryAsync<long>(sqlQuery, commandTimeout: 300)).ToList();
                actualIds.AddRange(tempIds);
            }

            var mappingsOfNotActualProducts = await _mapDb.ProductMaps
                .Where(c => !actualIds.Contains(c.LegacyId)).ToListAsync(cancellationToken);
            var erpGuids = mappingsOfNotActualProducts.Where(m => m.ErpGuid.HasValue).Select(m => m.ErpGuid.Value);

            await WriteGuidToFile(erpGuids);

            return new Unit();
        }

        private async Task WriteGuidToFile(IEnumerable<Guid> erpGuids)
        {
            await using StreamWriter sw =
                new StreamWriter($"{Path.GetFullPath($"ErpGuidsOfNotActualProducts_{DateTime.Now:MM_dd_yyyy}.txt")}", false,
                    System.Text.Encoding.Default);
            foreach (var guid in erpGuids)
            {
                sw.WriteLine(guid);
            }
        }
    }
}
