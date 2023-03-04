using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.SellingPrices;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class SellingPriceRepository : ILegacySellingPriceRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _dbConnection;
        private readonly IEnumerable<short> _allowedPrices;
        private Expression<Func<ProductEF, bool>> _productFilter;
        private readonly IProductMappingResolver _productMappingResolver;

        public SellingPriceRepository(LegacyDbContext db, ILegacyProductTypeRepository legacyProductTypeRepository, IProductMappingResolver productMappingResolver, LegacyDbConnection dbConnection)
        {
            _db = db;
            _productMappingResolver = productMappingResolver;
            _dbConnection = dbConnection;
            _allowedPrices = new List<short> {1, 2, 3, 5, 6, 8, 9, 11};
            var notUsedTypesIds = legacyProductTypeRepository.GetNotUsedTypesIds();
            _productFilter = PredicateBuilder.New<ProductEF>(p =>!notUsedTypesIds.Contains(p.ProductTypeId.Value));
        }

        public async IAsyncEnumerable<SellingPricePackage> GetChangedSellingPricesAsync(DateTime? changedAt, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (changedAt.HasValue)
            {
                var filter = PredicateBuilder.New<ProductEF>(true);
                filter = filter.And(p => p.DataLastPriceChange.HasValue && p.DataLastPriceChange > changedAt);

                await foreach (var p in GetAllAsync(filter, cancellationToken))
                {
                    yield return p;
                }

                yield break;
            }

            await foreach (var p in GetAllAsync(null, cancellationToken))
            {
                yield return p;
            }
        }

        public async IAsyncEnumerable<SellingPricePackage> GetInitialSellingPricesAsync(int? productId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<ProductEF, bool>> filter = null;
            if (productId.HasValue)
            {
                filter = PredicateBuilder.New<ProductEF>(p => p.Code == productId);
            }

            await foreach (var p in GetAllAsync(filter, cancellationToken))
            {
                yield return p;
            }
        }

        private async IAsyncEnumerable<SellingPricePackage> GetAllAsync(Expression<Func<ProductEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken, bool isTest = false)
        {
            var query = _db.Products
                .Where(_productFilter);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (isTest)
            {
                query = query.Take(100);
            }

            var productsEf = await query
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken: cancellationToken);

            await foreach (var p in GetPricePackages(productsEf, cancellationToken))
            {
                yield return p;
            }
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async IAsyncEnumerable<SellingPricePackage> GetPricePackages(IEnumerable<ProductEF> productsEf,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var prices = new List<SellingPricePackage>();
            foreach (var p in productsEf)
            {
                var newPackage = new SellingPricePackage();
                var productIdMap = await GetProductMapping(p.Code, p.NonCashProductId, cancellationToken);

                if (p.NonCashProductId.HasValue)
                {
                    var selectSqlQuery = @"select [КодТовара] as Code
                                         ,[DataLastPriceChange]
	                                     ,[beznal_tovID] as NonCashProductId
	                                     ,[ВалютаТовара] as Currency
	                                     ,[Цена0] as Price0
	                                     ,[Цена1] as Price1
	                                     ,[Цена2] as DistributorPrice
	                                     ,[Цена3] as RRPPrice
	                                     ,[Цена4] as SpecialPrice
	                                     ,[Цена5] as MinPrice
	                                     ,[ЦенаИ] as InternetPrice
                                         ,[SS] as FirstCost 
                                         ,[PriceMinBNuah] from [dbo].[Товары]
                                         where [КодТовара]=@ProductId";
                    var cashlessProduct = (await _dbConnection.Connection.QueryAsync(selectSqlQuery, new
                        {
                            ProductId = p.NonCashProductId
                        }))
                        .Select(e => new ProductEF
                        {
                            Code = p.Code,
                            NonCashProductId = (int?)e.NonCashProductId,
                            DataLastPriceChange = (DateTime?)e.DataLastPriceChange,
                            Currency = (byte?)e.Currency,
                            Price1 = (decimal?)e.Price1,
                            Price0 = (decimal?)e.Price0,
                            DistributorPrice = (decimal?)e.DistributorPrice,
                            RRPPrice = (decimal?)e.RRPPrice,
                            SpecialPrice = (decimal?)e.SpecialPrice,
                            MinPrice = (decimal?)e.MinPrice,
                            InternetPrice = (decimal?)e.InternetPrice,
                            FirstCost = (decimal?)e.FirstCost,
                            PriceMinBnuah = (decimal?)e.PriceMinBnuah
                        }).FirstOrDefault();

                    newPackage.CurrencyList.AddRange(MapCashlessProductPackageToDomain(cashlessProduct, productIdMap));
                    newPackage.CurrencyList.AddRange(MapCashProductPackageToDomain(p, productIdMap));
                }
                else
                {
                    var selectSqlQuery = @"select [КодТовара] as Code
                                         ,[DataLastPriceChange]
	                                     ,[beznal_tovID] as NonCashProductId
	                                     ,[ВалютаТовара] as Currency
	                                     ,[Цена0] as Price0
	                                     ,[Цена1] as Price1
	                                     ,[Цена2] as DistributorPrice
	                                     ,[Цена3] as RRPPrice
	                                     ,[Цена4] as SpecialPrice
	                                     ,[Цена5] as MinPrice
	                                     ,[ЦенаИ] as InternetPrice
                                         ,[SS] as FirstCost 
                                         ,[PriceMinBNuah] from [dbo].[Товары]
                                         where [beznal_tovID]=@ProductId";
                    var cashProduct = (await _dbConnection.Connection.QueryAsync(selectSqlQuery, new
                        {
                            ProductId = p.Code
                        }))
                        .Select(e => new ProductEF
                        {
                            Code = p.Code,
                            NonCashProductId = (int?)e.NonCashProductId,
                            DataLastPriceChange = (DateTime?)e.DataLastPriceChange,
                            Currency = (byte?)e.Currency,
                            Price1 = (decimal?)e.Price1,
                            Price0 = (decimal?)e.Price0,
                            DistributorPrice = (decimal?)e.DistributorPrice,
                            RRPPrice = (decimal?)e.RRPPrice,
                            SpecialPrice = (decimal?)e.SpecialPrice,
                            MinPrice = (decimal?)e.MinPrice,
                            InternetPrice = (decimal?)e.InternetPrice,
                            FirstCost = (decimal?)e.FirstCost,
                            PriceMinBnuah = (decimal?)e.PriceMinBnuah
                        }).FirstOrDefault();
                    if (cashProduct != null)
                    {
                        newPackage.CurrencyList.AddRange(MapCashlessProductPackageToDomain(p, productIdMap));
                        newPackage.CurrencyList.AddRange(MapCashProductPackageToDomain(cashProduct, productIdMap));
                    }
                    else
                    {
                        newPackage.CurrencyList.AddRange(MapCommonProductPackageToDomain(p, productIdMap));
                    }
                }

                newPackage.ProductId = productIdMap.ExternalId;
                prices.Add(newPackage);
            }

            if (prices.Any())
            {
                foreach (var price in prices)
                {
                    yield return price;
                }
            }
        }

        private async IAsyncEnumerable<SellingPrice> GetPrices(IEnumerable<ProductEF> productsEf,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var prices = new List<SellingPrice>();
            foreach (var p in productsEf)
            {
                if (p.NonCashProductId.HasValue)
                {
                    var selectSqlQuery = @"select [КодТовара] as Code
                                         ,[DataLastPriceChange]
	                                     ,[beznal_tovID] as NonCashProductId
	                                     ,[ВалютаТовара] as Currency
	                                     ,[Цена0] as Price0
	                                     ,[Цена1] as Price1
	                                     ,[Цена2] as DistributorPrice
	                                     ,[Цена3] as RRPPrice
	                                     ,[Цена4] as SpecialPrice
	                                     ,[Цена5] as MinPrice
	                                     ,[ЦенаИ] as InternetPrice
                                         ,[SS] as FirstCost 
                                         ,[PriceMinBNuah] from [dbo].[Товары]
                                         where [КодТовара]=@ProductId";
                    var cashlessProduct = (await _dbConnection.Connection.QueryAsync(selectSqlQuery, new
                    {
                        ProductId = p.NonCashProductId
                    }))
                        .Select(e => new ProductEF
                        {
                            Code = p.Code,
                            NonCashProductId = (int?)e.NonCashProductId,
                            DataLastPriceChange = (DateTime?)e.DataLastPriceChange,
                            Currency = (byte?)e.Currency,
                            Price1 = (decimal?)e.Price1,
                            Price0 = (decimal?)e.Price0,
                            DistributorPrice = (decimal?)e.DistributorPrice,
                            RRPPrice = (decimal?)e.RRPPrice,
                            SpecialPrice = (decimal?)e.SpecialPrice,
                            MinPrice = (decimal?)e.MinPrice,
                            InternetPrice = (decimal?)e.InternetPrice,
                            FirstCost = (decimal?)e.FirstCost,
                            PriceMinBnuah = (decimal?)e.PriceMinBnuah
                        }).FirstOrDefault();

                    prices.AddRange(await MapCashlessProductToDomain(cashlessProduct, cancellationToken));
                    prices.AddRange(await MapCashProductToDomain(p, cancellationToken));
                }
                else
                {
                    var selectSqlQuery = @"select [КодТовара] as Code
                                         ,[DataLastPriceChange]
	                                     ,[beznal_tovID] as NonCashProductId
	                                     ,[ВалютаТовара] as Currency
	                                     ,[Цена0] as Price0
	                                     ,[Цена1] as Price1
	                                     ,[Цена2] as DistributorPrice
	                                     ,[Цена3] as RRPPrice
	                                     ,[Цена4] as SpecialPrice
	                                     ,[Цена5] as MinPrice
	                                     ,[ЦенаИ] as InternetPrice
                                         ,[SS] as FirstCost 
                                         ,[PriceMinBNuah] from [dbo].[Товары]
                                         where [beznal_tovID]=@ProductId";
                    var cashProduct = (await _dbConnection.Connection.QueryAsync(selectSqlQuery, new
                    {
                        ProductId = p.Code
                    }))
                        .Select(e => new ProductEF
                        {
                            Code = p.Code,
                            NonCashProductId = (int?)e.NonCashProductId,
                            DataLastPriceChange = (DateTime?)e.DataLastPriceChange,
                            Currency = (byte?)e.Currency,
                            Price1 = (decimal?)e.Price1,
                            Price0 = (decimal?)e.Price0,
                            DistributorPrice = (decimal?)e.DistributorPrice,
                            RRPPrice = (decimal?)e.RRPPrice,
                            SpecialPrice = (decimal?)e.SpecialPrice,
                            MinPrice = (decimal?)e.MinPrice,
                            InternetPrice = (decimal?)e.InternetPrice,
                            FirstCost = (decimal?)e.FirstCost,
                            PriceMinBnuah = (decimal?)e.PriceMinBnuah
                        }).FirstOrDefault();
                    if (cashProduct != null)
                    {
                        prices.AddRange(await MapCashlessProductToDomain(p, cancellationToken));
                        prices.AddRange(await MapCashProductToDomain(cashProduct, cancellationToken));
                    }
                    else
                    {
                        prices.AddRange(await MapCommonProductToDomain(p, cancellationToken));
                    }
                }
            }

            if (prices.Any())
            {
                foreach (var price in prices)
                {
                    yield return price;
                }
            }
        }

        private async Task<IEnumerable<SellingPrice>> MapCashProductToDomain(ProductEF product, CancellationToken cancellationToken)
        {
            var productIdMap = await GetProductMapping(product.Code, product.NonCashProductId, cancellationToken);
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                sellingPrices.Add(new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cash
                ));
            }
            return sellingPrices;
        }

        private async Task<IEnumerable<SellingPrice>> MapCashlessProductToDomain(ProductEF product, CancellationToken cancellationToken)
        {
            var productIdMap = await GetProductMapping(product.Code, product.NonCashProductId, cancellationToken);
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                sellingPrices.Add(new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cashless
                ));
            }
            return sellingPrices;
        }

        private async Task<IEnumerable<SellingPrice>> MapCommonProductToDomain(ProductEF product, CancellationToken cancellationToken)
        {
            var productIdMap = await GetProductMapping(product.Code, product.NonCashProductId, cancellationToken);
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                var sellingPrice = new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cash
                );
                sellingPrices.Add(sellingPrice);

                var cashlessPrice = sellingPrice.GetCopyWith(PaymentTypes.Cashless);
                if (columnId == 8)
                {
                    cashlessPrice.ChangePrice(product.PriceMinBnuah ?? 0);
                }

                sellingPrices.Add(cashlessPrice);
            }
            return sellingPrices;
        }

        private IEnumerable<SellingPrice> MapCashProductPackageToDomain(ProductEF product, IdMap productIdMap)
        {
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                sellingPrices.Add(new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cash
                ));
            }
            return sellingPrices;
        }

        private IEnumerable<SellingPrice> MapCashlessProductPackageToDomain(ProductEF product, IdMap productIdMap)
        {
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                sellingPrices.Add(new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cashless
                ));
            }
            return sellingPrices;
        }

        private IEnumerable<SellingPrice> MapCommonProductPackageToDomain(ProductEF product, IdMap productIdMap)
        {
            var sellingPrices = new List<SellingPrice>();

            foreach (var columnId in _allowedPrices)
            {
                var sellingPrice = new SellingPrice(
                    date: product.DataLastPriceChange,
                    productId: productIdMap,
                    columnId: columnId,
                    price: product.GetPrice(columnId) ?? 0,
                    algorithm: "",
                    currency: product.Currency,
                    paymentType: PaymentTypes.Cash
                );
                sellingPrices.Add(sellingPrice);

                var cashlessPrice = sellingPrice.GetCopyWith(PaymentTypes.Cashless);
                if (columnId == 8)
                {
                    cashlessPrice.ChangePrice(product.PriceMinBnuah ?? 0);
                }

                sellingPrices.Add(cashlessPrice);
            }
            return sellingPrices;
        }
    }
}
