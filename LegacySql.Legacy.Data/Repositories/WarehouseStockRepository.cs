using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.WarehouseStock;
using LegacySql.Legacy.Data.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class WarehouseStockRepository : ILegacyWarehouseStockRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _dbConnection;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly List<int> _companiesIds = new List<int> { 1, 2, 33, 43, 101, 103, 111, 118 };

        public WarehouseStockRepository(LegacyDbContext db, AppDbContext mapDb, IProductMappingResolver productMappingResolver, LegacyDbConnection dbConnection)
        {
            _db = db;
            _mapDb = mapDb;
            _productMappingResolver = productMappingResolver;
            _dbConnection = dbConnection;
        }

        public async IAsyncEnumerable<ProductStock> GetChangedWarehouseStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var stock in GetAllAsync(lastChangedDate, notFullMappingIds, cancellationToken))
            {
                yield return stock;
            }
        }
        public async IAsyncEnumerable<ProductBNStock> GetChangedCompanyStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var stock in GetAllBNAsync(lastChangedDate, notFullMappingIds, cancellationToken))
            {
                yield return stock;
            }
        }

        public async IAsyncEnumerable<ProductStockReserved> GetChangedReservedWarehouseStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var stock in GetAllReservedAsync(lastChangedDate, notFullMappingIds, cancellationToken))
            {
                yield return stock;
            }
        }

        public async IAsyncEnumerable<ProductBNStockReserved> GetChangedReservedCompanyStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var stock in GetAllBNReservedAsync(lastChangedDate, notFullMappingIds, cancellationToken))
            {
                yield return stock;
            }
        }

        private async IAsyncEnumerable<ProductStock> GetAllAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken, bool isTest = false)
        {
            var procedure = "dbo.E21_pkg_get_products_actual_stock_changed_by_checkdate";
            var procedureParams = new
            {
                check_date = lastChangedDate,
                product_code_list = notFullMappingIds == null || !notFullMappingIds.Any() ? null : string.Join(",", notFullMappingIds)
            };
            var productStocks = await _dbConnection.Connection.QueryAsync<dynamic>(procedure, commandTimeout: 300, commandType: CommandType.StoredProcedure, param: procedureParams);

            //if (isTest)
            //{
                productStocks = productStocks.Take(100);
            //}

            var warehouseMap = await _mapDb.WarehouseMaps.AsNoTracking()
                .ToDictionaryAsync(wm => wm.LegacyId, wm => wm.ErpGuid, cancellationToken);

            foreach (var stock in productStocks)
            {
                var dictionary = new RouteValueDictionary(stock).ToDictionary(x => x.Key, y => y.Value);
                var dto = new ProductStocksEF
                {
                    ProductId = (int)dictionary["Product_Code"],
                    ProductCashlessId = (int?)dictionary["ProductCashlessId"],
                    ProductSumItemQty = (int?)dictionary["Product_SumItemQty"],
                    ProductSumOcItemQty = (int?)dictionary["Product_SumOcItemQty"],
                    VirtualWarehouseQuantity = (int)dictionary["10000"],
                    SupplierId = (int?)dictionary["SupplierId"],
                    FirstCost = (decimal?)dictionary["FirstCost"],
                    PriceMinBNuah = (decimal?)dictionary["PriceMinBNuah"],
                    WarehouseStocks = new Dictionary<int, int>()
                };
                dictionary.Remove("Product_row_id");
                dictionary.Remove("Product_Code");
                dictionary.Remove("ProductCashlessId");
                dictionary.Remove("Product_SumItemQty");
                dictionary.Remove("Product_SumOcItemQty");
                dictionary.Remove("10000");
                dictionary.Remove("SupplierId");
                dictionary.Remove("FirstCost");
                dictionary.Remove("PriceMinBNuah");

                foreach (var warehouse in dictionary)
                {
                    var warehouseQuantity = (int)warehouse.Value;
                    if (warehouseQuantity > 0)
                    {
                        dto.WarehouseStocks.Add(Convert.ToInt32(warehouse.Key), warehouseQuantity);
                    }
                }

                var (productMainSqlId, productErpGuid) =
                    await _productMappingResolver.ResolveMappingAsync(dto.ProductId, dto.ProductCashlessId,
                        cancellationToken);

                if (!dto.WarehouseStocks.Any())
                {
                    continue;
                }

                var warehouseStocks = dto.WarehouseStocks.Select(e => new WarehouseStock(new IdMap(e.Key,
                            warehouseMap.ContainsKey(e.Key)
                                ? warehouseMap[e.Key]
                                : null),
                        e.Value)).ToList();


                var supplierMap = dto.SupplierId.HasValue
                    ? await _mapDb.ClientMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyId == dto.SupplierId, cancellationToken)
                    : null;

                var (cashPrice, cashlessPrice) = await GetPrices(dto.ProductId, dto.ProductCashlessId,
                    dto.FirstCost, dto.PriceMinBNuah);

                yield return new ProductStock(
                    new IdMap(productMainSqlId, productErpGuid),
                    warehouseStocks,
                    supplierMap != null ? new IdMap(supplierMap.LegacyId, supplierMap.ErpGuid) : null,
                    cashPrice,
                    cashlessPrice
                );
            }
        }

        private async IAsyncEnumerable<ProductStockReserved> GetAllReservedAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken, bool isTest = false)
        {
            var procedure = "dbo.E21_pkg_get_products_actual_reserved_changed_by_checkdate";
            var procedureParams = new
            {
                check_date = lastChangedDate,
                product_code_list = notFullMappingIds == null || !notFullMappingIds.Any() ? null : string.Join(",", notFullMappingIds)
            };
            var productStocks = await _dbConnection.Connection.QueryAsync<dynamic>(procedure, commandTimeout: 300, commandType: CommandType.StoredProcedure, param: procedureParams);

            //if (isTest)
            //{
                productStocks = productStocks.Take(100);
            //}

            var warehouseMap = await _mapDb.WarehouseMaps.AsNoTracking()
                .ToDictionaryAsync(wm => wm.LegacyId, wm => wm.ErpGuid, cancellationToken);

            foreach (var stock in productStocks)
            {
                var dictionary = new RouteValueDictionary(stock).ToDictionary(x => x.Key, y => y.Value);
                var dto = new ProductStocksReservedEF
                {
                    ProductId = (int)dictionary["Product_Code"],
                    ProductCashlessId = (int?)dictionary["ProductCashlessId"],
                    ProductSumItemReservedQty = (int?)dictionary["Product_SumItemReservedQty"],
                    SupplierId = (int?)dictionary["SupplierId"],
                    FirstCost = (decimal?)dictionary["FirstCost"],
                    PriceMinBNuah = (decimal?)dictionary["PriceMinBNuah"],
                    WarehouseStocks = new Dictionary<int, int>()
                };
                dictionary.Remove("Product_row_id");
                dictionary.Remove("Product_Code");
                dictionary.Remove("ProductCashlessId");
                dictionary.Remove("Product_SumItemReservedQty");
                dictionary.Remove("SupplierId");
                dictionary.Remove("FirstCost");
                dictionary.Remove("PriceMinBNuah");

                foreach (var warehouse in dictionary)
                {
                    var warehouseQuantity = (int)warehouse.Value;
                    if (warehouseQuantity > 0)
                    {
                        dto.WarehouseStocks.Add(Convert.ToInt32(warehouse.Key), warehouseQuantity);
                    }
                }

                var (productMainSqlId, productErpGuid) =
                    await _productMappingResolver.ResolveMappingAsync(dto.ProductId, dto.ProductCashlessId,
                        cancellationToken);

                if (!dto.WarehouseStocks.Any())
                {
                    continue;
                }

                var warehouseStocks = dto.WarehouseStocks.Select(e => new WarehouseStock(new IdMap(e.Key,
                            warehouseMap.ContainsKey(e.Key)
                                ? warehouseMap[e.Key]
                                : null),
                        e.Value)).ToList();


                var supplierMap = dto.SupplierId.HasValue
                    ? await _mapDb.ClientMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyId == dto.SupplierId, cancellationToken)
                    : null;

                var (cashPrice, cashlessPrice) = await GetPrices(dto.ProductId, dto.ProductCashlessId,
                    dto.FirstCost, dto.PriceMinBNuah);

                yield return new ProductStockReserved(
                    new IdMap(productMainSqlId, productErpGuid),
                    warehouseStocks,
                    supplierMap != null ? new IdMap(supplierMap.LegacyId, supplierMap.ErpGuid) : null,
                    cashPrice,
                    cashlessPrice
                );
            }
        }

        private async IAsyncEnumerable<ProductBNStock> GetAllBNAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken, bool isTest = false)
        {
            var procedure = "dbo.E21_pkg_get_products_actual_bn_stock_changed_by_checkdate";
            var procedureParams = new
            {
                check_date = lastChangedDate,
                product_code_list = notFullMappingIds == null || !notFullMappingIds.Any() ? null : string.Join(",", notFullMappingIds)
            };
            var productStocks = await _dbConnection.Connection.QueryAsync<dynamic>(procedure, commandTimeout: 300, commandType: CommandType.StoredProcedure, param: procedureParams);

            //if (isTest)
            //{
            productStocks = productStocks.Take(100);
            //}

            foreach (var stock in productStocks)
            {
                var dictionary = new RouteValueDictionary(stock).ToDictionary(x => x.Key, y => y.Value);
                var dto = new CompanyStocksEF
                {
                    ProductId = (int)dictionary["Product_Code"],
                    ProductCashlessId = (int?)dictionary["ProductCashlessId"],
                    ProductSumItemQty = (int?)dictionary["Product_SumItemQty"],
                    SupplierId = (int?)dictionary["SupplierId"],
                    FirstCost = (decimal?)dictionary["FirstCost"],
                    PriceMinBNuah = (decimal?)dictionary["PriceMinBNuah"],
                    CompanyStocks = new Dictionary<int, int>()
                };
                dictionary.Remove("Product_row_id");
                dictionary.Remove("Product_Code");
                dictionary.Remove("ProductCashlessId");
                dictionary.Remove("Product_SumItemQty");
                dictionary.Remove("SupplierId");
                dictionary.Remove("FirstCost");
                dictionary.Remove("PriceMinBNuah");

                foreach (var warehouse in dictionary)
                {
                    var warehouseQuantity = (int)warehouse.Value;
                    if (warehouseQuantity > 0)
                    {
                        dto.CompanyStocks.Add(Convert.ToInt32(warehouse.Key), warehouseQuantity);
                    }
                }

                var (productMainSqlId, productErpGuid) =
                    await _productMappingResolver.ResolveMappingAsync(dto.ProductId, dto.ProductCashlessId,
                        cancellationToken);

                if (!dto.CompanyStocks.Any())
                {
                    continue;
                }

                var companies = await _db.Companies
                .Where(e => _companiesIds.Any(c => c == e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Okpo, cancellationToken);

                var companyStocks = dto.CompanyStocks.Select(e => new CompanyStock(companies.ContainsKey(e.Key) ? companies[e.Key] : e.Key.ToString(), e.Value)).ToList();

                var supplierMap = dto.SupplierId.HasValue
                ? await _mapDb.ClientMaps.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.LegacyId == dto.SupplierId, cancellationToken)
                : null;

                var (cashPrice, cashlessPrice) = await GetPrices(dto.ProductId, dto.ProductCashlessId,
                    dto.FirstCost, dto.PriceMinBNuah);

                yield return new ProductBNStock(
                    new IdMap(productMainSqlId, productErpGuid),
                    companyStocks,
                    supplierMap != null ? new IdMap(supplierMap.LegacyId, supplierMap.ErpGuid) : null,
                    cashPrice,
                    cashlessPrice
                );
            }
        }

        private async IAsyncEnumerable<ProductBNStockReserved> GetAllBNReservedAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken, bool isTest = false)
        {
            var procedure = "dbo.E21_pkg_get_products_actual_bn_reserved_changed_by_checkdate";
            var procedureParams = new
            {
                check_date = lastChangedDate,
                product_code_list = notFullMappingIds == null || !notFullMappingIds.Any() ? null : string.Join(",", notFullMappingIds)
            };
            var productStocks = await _dbConnection.Connection.QueryAsync<dynamic>(procedure, commandTimeout: 300, commandType: CommandType.StoredProcedure, param: procedureParams);

            //if (isTest)
            //{
            productStocks = productStocks.Take(100);
            //}

            foreach (var stock in productStocks)
            {
                var dictionary = new RouteValueDictionary(stock).ToDictionary(x => x.Key, y => y.Value);
                var dto = new CompanyStocksReservedEF
                {
                    ProductId = (int)dictionary["Product_Code"],
                    ProductCashlessId = (int?)dictionary["ProductCashlessId"],
                    ProductSumItemReservedQty = (int?)dictionary["Product_SumItemReservedQty"],
                    SupplierId = (int?)dictionary["SupplierId"],
                    FirstCost = (decimal?)dictionary["FirstCost"],
                    PriceMinBNuah = (decimal?)dictionary["PriceMinBNuah"],
                    CompanyStocks = new Dictionary<int, int>()
                };
                dictionary.Remove("Product_row_id");
                dictionary.Remove("Product_Code");
                dictionary.Remove("ProductCashlessId");
                dictionary.Remove("Product_SumItemReservedQty");
                dictionary.Remove("SupplierId");
                dictionary.Remove("FirstCost");
                dictionary.Remove("PriceMinBNuah");

                foreach (var warehouse in dictionary)
                {
                    var warehouseQuantity = (int)warehouse.Value;
                    if (warehouseQuantity > 0)
                    {
                        dto.CompanyStocks.Add(Convert.ToInt32(warehouse.Key), warehouseQuantity);
                    }
                }

                var (productMainSqlId, productErpGuid) =
                    await _productMappingResolver.ResolveMappingAsync(dto.ProductId, dto.ProductCashlessId,
                        cancellationToken);

                if (!dto.CompanyStocks.Any())
                {
                    continue;
                }

                var companies = await _db.Companies
                .Where(e => _companiesIds.Any(c => c == e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.Okpo, cancellationToken);

                var companyStocks = dto.CompanyStocks.Select(e => new CompanyStock(companies.ContainsKey(e.Key) ? companies[e.Key] : e.Key.ToString(), e.Value)).ToList();

                var supplierMap = dto.SupplierId.HasValue
                    ? await _mapDb.ClientMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyId == dto.SupplierId, cancellationToken)
                    : null;

                var (cashPrice, cashlessPrice) = await GetPrices(dto.ProductId, dto.ProductCashlessId,
                    dto.FirstCost, dto.PriceMinBNuah);

                yield return new ProductBNStockReserved(
                    new IdMap(productMainSqlId, productErpGuid),
                    companyStocks,
                    supplierMap != null ? new IdMap(supplierMap.LegacyId, supplierMap.ErpGuid) : null,
                    cashPrice,
                    cashlessPrice
                );
            }
        }

        //private async IAsyncEnumerable<ProductStock> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken,
        //    bool isTest = false)
        //{
        //    var selectStocksSqlQuery = @"select prStock.[КодТовара] as ProductId,
        //                                pr.[beznal_tovID] as ProductCashlessId,
        //                                [1] as Company1,
        //                                [2] as Company2,
        //                                [33] as Company33,
        //                                [43] as Company43,
        //                                [101] as Company101,
        //                                [103] as Company103,
        //                                [111] as Company111,
        //                                [118] as Company118,
        //                                [skladID] as WarehouseId,
        //                                [Qfree] as WarehouseQuantity,
        //		prStock.[suplID] as SupplierId,
        //		[SS] as FirstCost,
        //		[PriceMinBNuah]
        //                                from dbo.lSQL_v_Товары_aggregation_items_with_bn_stock as prStock
        //                                left join dbo.[SkladFree]
        //                                on prStock.КодТовара = dbo.[SkladFree].tovID
        //                                left join dbo.[lSQL_v_Товары] as pr
        //                                on pr.[КодТовара] = dbo.[SkladFree].tovID";

        //    var productStocks = (await _dbConnection.Connection.QueryAsync<ProductStocksEF>(selectStocksSqlQuery, commandTimeout: 300))
        //        .GroupBy(e => new {e.ProductId, e.ProductCashlessId});

        //    if (isTest)
        //    {
        //        productStocks = productStocks.Take(100);
        //    }

        //    var warehouseMap = await _mapDb.WarehouseMaps.AsNoTracking()
        //        .ToDictionaryAsync(wm => wm.LegacyId, wm => wm.ErpGuid, cancellationToken);
        //    var companies = await _db.Companies
        //        .Where(e => _companiesIds.Any(c => c == e.Id))
        //        .ToDictionaryAsync(e => e.Id, e => e.Okpo, cancellationToken);
        //    var suppliers = productStocks.ToDictionary(e => e.Key, e => e.First().SupplierId);

        //    foreach (var stock in productStocks)
        //    {
        //        var (productMainSqlId, productErpGuid) =
        //            await _productMappingResolver.ResolveMappingAsync(stock.Key.ProductId, stock.Key.ProductCashlessId,
        //                cancellationToken);

        //        var warehouseStocks = stock.Where(e => e.WarehouseId.HasValue && e.WarehouseQuantity.HasValue)
        //            .Select(e => new WarehouseStock(new IdMap(e.WarehouseId.Value,
        //                    warehouseMap.ContainsKey(e.WarehouseId.Value)
        //                        ? warehouseMap[(int) e.WarehouseId]
        //                        : null),
        //                e.WarehouseQuantity.Value)).ToList();

        //        var company1Stock = stock.FirstOrDefault(e => e.Company1.HasValue && e.Company1 != 0);
        //        var company2Stock = stock.FirstOrDefault(e => e.Company2.HasValue && e.Company2 != 0);
        //        var company33Stock = stock.FirstOrDefault(e => e.Company33.HasValue && e.Company33 != 0);
        //        var company43Stock = stock.FirstOrDefault(e => e.Company43.HasValue && e.Company43 != 0);
        //        var company101Stock = stock.FirstOrDefault(e => e.Company101.HasValue && e.Company101 != 0);
        //        var company103Stock = stock.FirstOrDefault(e => e.Company103.HasValue && e.Company103 != 0);
        //        var company111Stock = stock.FirstOrDefault(e => e.Company111.HasValue && e.Company111 != 0);
        //        var company118Stock = stock.FirstOrDefault(e => e.Company118.HasValue && e.Company118 != 0);

        //        var companyStocks = new List<CompanyStock>();
        //        if (company1Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[1], company1Stock.Company1.Value));
        //        }

        //        if (company2Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[2], company2Stock.Company2.Value));
        //        }

        //        if (company33Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[33], company33Stock.Company33.Value));
        //        }

        //        if (company43Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[43], company43Stock.Company43.Value));
        //        }

        //        if (company101Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[101], company101Stock.Company101.Value));
        //        }

        //        if (company103Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[103], company103Stock.Company103.Value));
        //        }

        //        if (company111Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[111], company111Stock.Company111.Value));
        //        }

        //        if (company118Stock != null)
        //        {
        //            companyStocks.Add(new CompanyStock(companies[118], company118Stock.Company118.Value));
        //        }

        //        if (!warehouseStocks.Any() && !companyStocks.Any())
        //        {
        //            continue;
        //        }

        //        var supplierMap = suppliers[stock.Key].HasValue
        //            ? await _mapDb.ClientMaps.AsNoTracking()
        //                .FirstOrDefaultAsync(m => m.LegacyId == suppliers[stock.Key], cancellationToken)
        //            : null;

        //        var (cashPrice, cashlessPrice) = await GetPrices(stock.Key.ProductId, stock.Key.ProductCashlessId,
        //            stock.First().FirstCost, stock.First().PriceMinBNuah);

        //        yield return new ProductStock(
        //            new IdMap(productMainSqlId, productErpGuid),
        //            warehouseStocks,
        //            supplierMap != null ? new IdMap(supplierMap.LegacyId, supplierMap.ErpGuid) : null,
        //            cashPrice,
        //            cashlessPrice
        //        );
        //    }
        //}

        private async Task<(decimal cashPrice, decimal cashlessPrice)> GetPrices(int productId, int? cashlessProductId,
            decimal? firstCost, decimal? priceMinBNuah)
        {
            if (cashlessProductId.HasValue)
            {
                var selectSqlQuery = @"select [SS] as FirstCost from [dbo].[Товары]
                                     where [КодТовара]=@ProductId";
                var cashlessProductPrice = (await _dbConnection.Connection.QueryAsync<decimal?>(selectSqlQuery, new
                {
                    ProductId = cashlessProductId
                })).FirstOrDefault();

                return (firstCost ?? 0, cashlessProductPrice ?? 0);
            }
            else
            {
                var selectSqlQuery = @"select [SS] as FirstCost from [dbo].[Товары]
                                     where [beznal_tovID]=@ProductId";
                var cashProduct = (await _dbConnection.Connection.QueryAsync(selectSqlQuery, new
                {
                    ProductId = productId
                }))
                    .Select(e => new
                    {
                        Code = productId,
                        FirstCost = (decimal?)e.FirstCost
                    }).FirstOrDefault();
                return cashProduct != null
                    ? (cashProduct.FirstCost ?? 0, firstCost ?? 0)
                    : (firstCost ?? 0, priceMinBNuah ?? 0);
            }
        }
    }
}