using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.Products;
using LegacySql.Domain.SellingPrices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.PriceLists.Get
{
    public class GetPriceListQueryHandler : IRequestHandler<GetPriceListQuery, IEnumerable<PriceListItemDto>>
    {
        private readonly IDbConnection _legacySqlConnection;
        private readonly AppDbContext _appDb;

        public GetPriceListQueryHandler(IDbConnection legacySqlConnection, AppDbContext appDb)
        {
            _legacySqlConnection = legacySqlConnection;
            _appDb = appDb;
        }

        public async Task<IEnumerable<PriceListItemDto>> Handle(GetPriceListQuery query, CancellationToken cancellationToken)
        {
            var sqlParams = new
            {
                ProductManager = await GetProductManagerLogin(query.ProductManagerId, cancellationToken),
                ProductTypes = await GetProductTypeIds(query.ProductTypeIds, cancellationToken),
                Manufacturer = await GetManufacturerTitle(query.ManufacturerId, cancellationToken),
            };
            
            var sqlBuilder = new SqlBuilder();

            var selector = sqlBuilder.AddTemplate(
                $@"SELECT dbo.Товары.КодТовара as ProductId, Цена3 as RetailPrice,                                 
                    dbo.nzi(Товары.нал_компы) AS Computers, dbo.Товары.{query.PriceColumn.Title} as Price,
                    dbo.star(Товары.нал_ф - Товары.нал_компы-нал_Резерв,'Ожидается') AS Cash, replace(inet,'#','') as Inet 
                FROM dbo.Товары 
                /**where**/
                ORDER BY dbo.Товары.КодТипа, dbo.Товары.подтип, dbo.Товары.Марка");
            
            if (!string.IsNullOrEmpty(sqlParams.ProductManager))
            {
                sqlBuilder.Where("Товары.ProductManager = @ProductManager", new { sqlParams.ProductManager });
            }

            if (sqlParams.ProductTypes != null && sqlParams.ProductTypes.Any())
            {
                sqlBuilder.Where("Товары.КодТипа IN @ProductTypes ", new { sqlParams.ProductTypes });
            }

            if (!string.IsNullOrEmpty(sqlParams.Manufacturer))
            {
                sqlBuilder.Where("Товары.manufacture = @Manufacturer", new { sqlParams.Manufacturer });
            }
            
            var result = (await _legacySqlConnection.QueryAsync<LegacyPriceListItemRequestResult>(selector.RawSql, selector.Parameters)).ToList();

            Dictionary<long, Guid> productIds = await GetProductIds(result.Select(r => r.ProductId).ToList(), cancellationToken);

            return result.Select(p => new PriceListItemDto
            {
                ProductId = productIds.TryGetValue(p.ProductId, out var productId) ? (Guid?)productId : null,
                RetailPrice = p.RetailPrice,
                Computers = p.Computers,
                Price = p.Price,
                Cash = p.Cash,
                Inet = p.Inet,
            }).ToList();
        }

        private async Task<IEnumerable<int>> GetProductTypeIds(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            return ids == null
                ? null
                : await _appDb.ProductTypeMaps.AsNoTracking()
                    .Where(p => p.ErpGuid.HasValue && ids.Contains(p.ErpGuid.Value))
                    .Select(p => p.LegacyId)
                    .ToListAsync(cancellationToken);
        }

        private async Task<string> GetProductManagerLogin(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var map = await _appDb.EmployeeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ErpGuid == id, cancellationToken);
            if (map == null)
            {
                throw new ArgumentException($"Маппинг сотрудника id:{id} не найден\n");
            }

            var sql = @"SELECT uuu
                        FROM dbo.Сотрудники
                        WHERE КодСотрудника = @Id";
            var productManager = await _legacySqlConnection.QueryFirstOrDefaultAsync<string>(sql, new
            {
                Id = map.LegacyId,
            });

            return productManager;
        }

        private async Task<string> GetManufacturerTitle(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var map = await _appDb.ManufacturerMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ErpGuid == id, cancellationToken);
            if (string.IsNullOrEmpty(map?.LegacyTitle))
            {
                throw new ArgumentException($"Маппинг производителя id:{id} не найден\n");
            }

            return map.LegacyTitle;
        }

        private async Task<Dictionary<long, Guid>> GetProductIds(List<long> ids, CancellationToken cancellationToken)
        {
            Dictionary<long, Guid> products = new Dictionary<long, Guid>();

            var maps = await _appDb.ProductMaps.AsNoTracking()
                .Where(p => ids.Contains(p.LegacyId))
                .ToListAsync(cancellationToken);
            foreach (var map in maps)
            {
                if (!map.ErpGuid.HasValue)
                {
                    throw new ArgumentException($"Маппинг товара id:{map.LegacyId} не найден\n");
                }

                products[map.LegacyId] = map.ErpGuid.Value;
            }

            return products;
        }

        private class LegacyPriceListItemRequestResult
        {
            public long ProductId { get; set; }
            public decimal RetailPrice { get; set; }
            public string Computers { get; set; }
            public decimal Price { get; set; }
            public string Cash { get; set; }
            public string Inet { get; set; }
        }
    }
}