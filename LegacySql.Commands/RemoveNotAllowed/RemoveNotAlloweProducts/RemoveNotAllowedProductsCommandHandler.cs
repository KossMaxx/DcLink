using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Commands.RemoveNotAllowed.RemoveNotAllowedProducts
{
    public class RemoveNotAllowedProductsCommandHandler : IRequestHandler<RemoveNotAllowedProductsCommand>
    {
        private readonly IDbConnection _db;
        private readonly AppDbContext _mapDb;

        public RemoveNotAllowedProductsCommandHandler(IDbConnection db, AppDbContext mapDb, IBus bus)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<Unit> Handle(RemoveNotAllowedProductsCommand command, CancellationToken cancellationToken)
        {
            var countQuery = "select count(*) from lSQL_v_Товары";
            var count = await _db.QueryFirstOrDefaultAsync<int>(countQuery, commandTimeout: 300);
            var portion = 10000;
            var cycleLimitation = (double)count / portion;
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

            await RemoveNotAllowedMappings(mappingsOfNotActualProducts, cancellationToken);

            return new Unit();
        }

        private async Task RemoveNotAllowedMappings(List<ProductMapEF> mappingsOfNotAllowedProducts, CancellationToken cancellationToken)
        {
            _mapDb.ProductMaps.RemoveRange(mappingsOfNotAllowedProducts);
            await _mapDb.SaveChangesAsync(cancellationToken);
        }
    }
}

