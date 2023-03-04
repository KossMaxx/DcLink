using Dapper;
using LegacySql.Data;
using LegacySql.Domain.ProductMoving;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.ProductMovings
{
    public class ProductMovingRepository : ILegacyProductMovingRepository
    {
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _sqlConnection;

        public ProductMovingRepository(LegacyDbConnection sqlConnection, AppDbContext mapDb)
        {
            _sqlConnection = sqlConnection;
            _mapDb = mapDb;
        }

        private async Task<IEnumerable<ProductMovingData>> GetProductsFromDb(DateTime? changedAt = null, IEnumerable<int> idsFilter = null)
        {
            var selectSqlQuery = new StringBuilder($@"select 
                                                      prn.prn_ID as Id, 
                                                      prn.дата as Date, 
                                                      prn.юзер as CreatorUsername, 
                                                      prn.откуда as OutWarehouseId, 
                                                      prn.куда as InWarehouseId, 
                                                      prn.прим as Description, 
                                                      company.okpo as Okpo, 
                                                      empl.КодСотрудника as CreatorId,
                                                      prod.КодТовара as ProductId, 
                                                      move.Количество as Amount, 
                                                      prod.SS as Price,
                                                      prn.timeO as ShippedDate, 
                                                      prn.timeS as ForShipmentDate,
                                                      prn.modified_at as ChangedAt,
                                                      prn.[ф] as IsAccepted,
                                                      prn.[O] as IsForShipment,
                                                      prn.[S] as IsShipped
                                                      FROM dbo.PRN prn
                                                      inner join dbo.move move on move.prn_ID = prn.prn_ID
                                                      inner join dbo.Товары prod on prod.КодТовара = move.КодТовара
                                                      inner join dbo.OOO company on company.ID = prn.spdfl
                                                      inner join dbo.Склады wh on wh.sklad_ID = prn.куда
                                                      left join dbo.Сотрудники empl on empl.uuu = prn.юзер 
                                                      where prn.O = 1 and prn.S = 1 and prn.ф = 0 and wh.RMA = 1");

            if (changedAt.HasValue)
            {
                selectSqlQuery.Append($@" and cast(prn.modified_at as date) > cast({changedAt} as date)");
            }

            if (idsFilter.Any())
            {
                selectSqlQuery.Append($" or prn.prn_ID in ({string.Join(",", idsFilter)})");
            }

            var movingData = await _sqlConnection.Connection.QueryAsync<ProductMovingData, ProductMovingItemData, ProductMovingData>(selectSqlQuery.ToString(),
                map: (moving, item) =>
                {
                    moving.Item = item;
                    return moving;
                },
                splitOn: "ProductId",
                commandTimeout: 600
                );

            return movingData;
        }

        public async IAsyncEnumerable<ProductMoving> GetChangedProductMovingsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken)
        {
            var productMovingData = await GetProductsFromDb(lastChangedDate, notFullMappingIds);
            if (!productMovingData.Any())
            {
                yield return null;
            }

            var maps = await Maps.Create(productMovingData, _mapDb, cancellationToken);
            var productMovingMapper = new ProductMovingMapper(maps.ProductMovingMap, maps.ProductMap, maps.WarehouseMap, maps.CreatorMap);

            foreach (var productMoving in productMovingData.GroupBy(e => e.Id)
                                                .Select(productMovingGroup =>
                                                {
                                                    var masterProduct = productMovingGroup.First();
                                                    var productMoving = productMovingMapper.Map(masterProduct, productMovingGroup.ToList());

                                                    return productMoving;
                                                }))
            {
                yield return productMoving;
            }
        }

        public async Task<ProductMoving> GetProductMovingAsync(int id, CancellationToken cancellationToken)
        {
            var proceduresMovingIdsParam = new List<int> { id };
            var productMovingData = await GetProductsFromDb(null, proceduresMovingIdsParam);

            var maps = await Maps.Create(productMovingData, _mapDb, cancellationToken);
            var productMovingMapper = new ProductMovingMapper(maps.ProductMovingMap, maps.ProductMap, maps.WarehouseMap, maps.CreatorMap);

            if(!productMovingData.Any())
            {
                return null;
            }
            
            var masterProductMoving = productMovingData.GroupBy(e=>e.Id).First(e=>e.Key == id);
            return productMovingMapper.Map(masterProductMoving.First(), masterProductMoving.ToList());
        }

        private class Maps
        {
            public IDictionary<int, Guid?> ProductMovingMap { get; set; }
            public IDictionary<int, Guid?> ProductMap { get; set; }
            public IDictionary<int, Guid?> WarehouseMap { get; set; }
            public IDictionary<int, Guid?> CreatorMap { get; set; }

            public static async Task<Maps> Create(
                IEnumerable<ProductMovingData> data,
                AppDbContext mapDb,
                CancellationToken cancellationToken)
            {
                var uniqProductMovingIds = new List<int>();
                var uniqProductIds = new List<int>();
                var uniqWarehouseIds = new List<int>();
                var uniqCreatorIds = new List<int>();

                foreach (var moving in data)
                {
                    uniqProductMovingIds.Add(moving.Id);

                    if (moving.Item != null)
                    {
                        uniqProductIds.Add(moving.Item.ProductId);
                    }

                    if(moving.OutWarehouseId > 0)
                    {
                        uniqWarehouseIds.Add(moving.OutWarehouseId);
                    }
                    if (moving.InWarehouseId > 0)
                    {
                        uniqWarehouseIds.Add(moving.InWarehouseId);
                    }

                    if (!string.IsNullOrEmpty(moving.CreatorUsername) && moving.CreatorId.HasValue)
                    {
                        uniqCreatorIds.Add(moving.CreatorId.Value);
                    }
                }

                uniqProductMovingIds = uniqProductMovingIds.Distinct().ToList();
                uniqProductIds = uniqProductIds.Distinct().ToList();
                uniqWarehouseIds = uniqWarehouseIds.Distinct().ToList();
                uniqCreatorIds = uniqCreatorIds.Distinct().ToList();

                var productMovingMap = await mapDb.ProductMovingMaps.AsNoTracking()
                    .Where(t=> uniqProductMovingIds.Contains(t.LegacyId))
                    .ToDictionaryAsync(t => t.LegacyId, t => t.ErpGuid, cancellationToken);

                var productMap = await mapDb.ProductMaps.AsNoTracking()
                    .Where(cm => uniqProductIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var warehouseMap = await mapDb.WarehouseMaps.AsNoTracking()
                    .Where(w => uniqWarehouseIds.Contains(w.LegacyId))
                    .ToDictionaryAsync(w => w.LegacyId, w => w.ErpGuid, cancellationToken);

                var creatorMap = await mapDb.EmployeeMaps.AsNoTracking()
                    .Where(cm => uniqCreatorIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(c => c.LegacyId, c => c.ErpGuid, cancellationToken);
                               

                return new Maps
                {
                    ProductMovingMap = productMovingMap,
                    ProductMap = productMap,
                    WarehouseMap = warehouseMap,
                    CreatorMap = creatorMap
                };
            }
        }
    }
}
