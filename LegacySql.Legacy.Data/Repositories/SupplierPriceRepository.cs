using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierPrice;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class SupplierPriceRepository : ILegacySupplierPriceRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;
        private Dictionary<string, NotInStockStatusEF> _notInStockStatuses;
        private Dictionary<int, SupplierCurrencyRateEF> _supplierCurrencyRates;
        private readonly IProductMappingResolver _productMappingResolver;

        public SupplierPriceRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion, IProductMappingResolver productMappingResolver)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
        }

        public async Task<IEnumerable<SupplierPrice>> GetAllAsync(IEnumerable<int> filteredProductsIds, DateTime? date, CancellationToken cancellationToken)
        {
            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var pricesQuery =  _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                );
            
            if (date.HasValue)
            {
                pricesQuery = pricesQuery.Where(p => p.Date.HasValue && p.Date.Value.Date == date.Value.Date);
            }
                
            var pricesEf = await pricesQuery
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEf);
            return await MapToDomain(filteringPrices, cancellationToken);
        }

        public async Task<IEnumerable<SupplierPricePackage>> GetAllPackagesAsync(IEnumerable<int> filteredProductsIds, DateTime? date, CancellationToken cancellationToken)
        {
            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var pricesQuery = _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                );

            if (date.HasValue)
            {
                pricesQuery = pricesQuery.Where(p => p.Date.HasValue && p.Date.Value.Date == date.Value.Date);
            }

            var pricesEf = await pricesQuery
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEf);
            return await MapToPackage(filteringPrices, cancellationToken);
        }

        public async Task<IEnumerable<SupplierPrice>> GetInitialSellingPricesAsync(IEnumerable<int> filteredProductsIds, CancellationToken cancellationToken)
        {
            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var pricesEf = await _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
#if DEBUG
                .Take(1000)
#endif
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                )
                .ToListAsync(cancellationToken: cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEf).ToList();

            var initialPrices = filteringPrices.GroupBy(p => new { p.ProductId, p.SupplierId }).Select(g => g.OrderByDescending(e => e.Date).First());

            return await MapToDomain(initialPrices, cancellationToken);
        }

        public async Task<IEnumerable<SupplierPricePackage>> GetInitialSellingPricePackagesAsync(IEnumerable<int> filteredProductsIds, CancellationToken cancellationToken)
        {
            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var pricesEf = await _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
#if DEBUG
                .Take(1000)
#endif
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                )
                .ToListAsync(cancellationToken: cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEf).ToList();

            var initialPrices = filteringPrices.GroupBy(p => new { p.ProductId, p.SupplierId }).Select(g => g.OrderByDescending(e => e.Date).First());

            return await MapToPackage(initialPrices, cancellationToken);
        }

        private async Task Update_NotInStockStatuses_SupplierCurrencyRates(CancellationToken cancellationToken)
        {
            _notInStockStatuses = (await _db.NotInStockStatuses
                    .ToListAsync(cancellationToken))
                .GroupBy(s => s.Name)
                .ToDictionary(s => s.Key, s => s.First());

            _supplierCurrencyRates = (await _db.SupplierCurrencyRates
                    .Where(r => r.ClientId.HasValue)
                    .ToListAsync(cancellationToken))
                .GroupBy(r => r.ClientId.Value)
                .ToDictionary(r => r.Key, r => r.OrderByDescending(i => i.Date).First());
        }

        public async Task<(IEnumerable<SupplierPrice> supplierPrice, DateTime? lastDate)> GetChangedSupplierPricesAsync(
            IEnumerable<int> filteredProductsIds, DateTime? changedAt, IEnumerable<int> notFullMappingIds,
            CancellationToken cancellationToken)
        {
            if (!changedAt.HasValue)
            {
                var allPrices = await GetAllAsync(filteredProductsIds, null, cancellationToken);
                return (allPrices, GetLastDate(allPrices));
            }

            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var changedRatesClientIds = _supplierCurrencyRates
                .Where(r => r.Value.Date >= changedAt)
                .Select(r => r.Value.ClientId);

            var pricesEfByDate = await _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
                .Where(sp => sp.Date > changedAt || changedRatesClientIds.Any(id => id == sp.SupplierId))
#if DEBUG
                .Take(1000)
#endif
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                )
                .ToListAsync(cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEfByDate).ToList();
            
            var prices = await MapToDomain(filteringPrices, cancellationToken);

            var lastDate = GetLastDate(prices);

            var pricesByNotFullMapping = await GetByNotFullMapping(notFullMappingIds, cancellationToken);
            prices.AddRange(pricesByNotFullMapping);

            return (prices, lastDate);
        }

        public async Task<(IEnumerable<SupplierPricePackage> supplierPrice, DateTime? lastDate)> GetChangedSupplierPricePackagesAsync(
           IEnumerable<int> filteredProductsIds, DateTime? changedAt, IEnumerable<int> notFullMappingIds,
           CancellationToken cancellationToken)
        {
            if (!changedAt.HasValue)
            {
                var allPrices = await GetAllPackagesAsync(filteredProductsIds, null, cancellationToken);
                return (allPrices, GetLastPackageDate(allPrices));
            }

            await Update_NotInStockStatuses_SupplierCurrencyRates(cancellationToken);

            var changedRatesClientIds = _supplierCurrencyRates
                .Where(r => r.Value.Date >= changedAt)
                .Select(r => r.Value.ClientId);

            var pricesEfByDate = await _db.SupplierActualPrices
                .Where(p => filteredProductsIds.Contains(p.ProductId))
                .Where(sp => sp.Date > changedAt || changedRatesClientIds.Any(id => id == sp.SupplierId))
#if DEBUG
                .Take(1000)
#endif
                .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                    new SupplierActualPriceEF
                    {
                        Id = sp.Id,
                        Date = sp.Date,
                        ProductId = sp.ProductId,
                        Product = sp.Product,
                        SupplierId = sp.SupplierId,
                        Price = sp.Price,
                        PriceRetail = sp.PriceRetail,
                        PriceDialer = sp.PriceDialer,
                        SupplierProductCode = sp.SupplierProductCode,
                        Monitor = sp.Monitor,
                        IsInStock = sp.IsInStock,
                        Currency = c.BalanceCurrencyId,
                    }
                )
                .ToListAsync(cancellationToken);

            var filteringPrices = FilterNullPrices(pricesEfByDate).ToList();

            var prices = await MapToDomain(filteringPrices, cancellationToken);

            var lastDate = GetLastDate(prices);

            var pricesByNotFullMapping = await GetByNotFullMapping(notFullMappingIds, cancellationToken);
            prices.AddRange(pricesByNotFullMapping);

            return (prices.GroupBy(p => p.SupplierId).Select(np => new SupplierPricePackage() { Supplier = np.Key.ExternalId, SupplierPrices = np.ToList() }).ToList(), lastDate);
        }

        private async Task<List<SupplierPrice>> MapToDomain(IEnumerable<SupplierActualPriceEF> pricesEf, CancellationToken cancellationToken)
        {
            var mappedClientsIds = await GetClientsIdsForFilter(cancellationToken);

            var prices = (pricesEf.Where(p => mappedClientsIds.Any(id => id == p.SupplierId))
                .Select(async p => await MapToDomainForCash(p, cancellationToken))
                .Select(p => p.Result)).ToList();

            var cashlessClientsIds = _supplierCurrencyRates
                .Where(r => r.Value.Date.HasValue && r.Value.Date.Value.Date == DateTime.Today ||
                            r.Value.Date.Value.Date == DateTime.Today.AddDays(-1))
                .Select(r => r.Key);

            var cashlessPrices = (pricesEf.Where(p => cashlessClientsIds.Any(id => id == p.SupplierId))
                .Select(async p => await MapToDomainForCashless(p, cancellationToken))
                .Select(p => p.Result));

            prices.AddRange(cashlessPrices);
            return prices;
        }

        private async Task<List<SupplierPricePackage>> MapToPackage(IEnumerable<SupplierActualPriceEF> pricesEf, CancellationToken cancellationToken)
        {
            var mappedClientsIds = await GetClientsIdsForFilter(cancellationToken);

            var prices = (pricesEf.Where(p => mappedClientsIds.Any(id => id == p.SupplierId))
                .Select(async p => await MapToDomainForCash(p, cancellationToken))
                .Select(p => p.Result)).ToList();

            var cashlessClientsIds = _supplierCurrencyRates
                .Where(r => r.Value.Date.HasValue && r.Value.Date.Value.Date == DateTime.Today ||
                            r.Value.Date.Value.Date == DateTime.Today.AddDays(-1))
                .Select(r => r.Key);

            var cashlessPrices = (pricesEf.Where(p => cashlessClientsIds.Any(id => id == p.SupplierId))
                .Select(async p => await MapToDomainForCashless(p, cancellationToken))
                .Select(p => p.Result));
            prices.AddRange(cashlessPrices);
            return prices.GroupBy(p => p.SupplierId).Select(np => new SupplierPricePackage() { Supplier = np.Key.ExternalId, SupplierPrices = np.ToList() }).ToList();
        }

        private async Task<SupplierPrice> MapToDomainForCashA(SupplierActualPriceEF price, CancellationToken cancellationToken)
        {
            var supplierMap = await _mapDb.ClientMaps.FirstOrDefaultAsync(m => m.LegacyId == price.SupplierId,
                cancellationToken: cancellationToken);

            var checkUrlResult = !string.IsNullOrEmpty(price.SupplierProductCode) && CheckUrl(price.SupplierProductCode);

            var currencyRate = price.SupplierId.HasValue && _supplierCurrencyRates.ContainsKey(price.SupplierId.Value)
                ? _supplierCurrencyRates[price.SupplierId.Value]
                : null;

            return new SupplierPrice(
                price.Id,
                price.Date,
                await GetProductMapping(price.Product.Code, price.Product.NonCashProductId, cancellationToken),
                price.SupplierId.HasValue ? new IdMap(price.SupplierId.Value, supplierMap?.ErpGuid) : null,
                price.Price,
                !checkUrlResult  ? price.SupplierProductCode : "",
                checkUrlResult ? price.SupplierProductCode : "",
                price.Monitor,
                price.PriceRetail,
                price.PriceDialer,
                price.Currency,
                price.IsInStock != null && !_notInStockStatuses.ContainsKey(price.IsInStock),
                PaymentTypes.Cash,
                currencyRate?.RateNal,
                price.IsInStock,
                currencyRate?.Date
            );
        }

        private async Task<SupplierPrice> MapToDomainForCash(SupplierActualPriceEF price, CancellationToken cancellationToken)
        {
            var supplierMap = await _mapDb.ClientMaps.FirstOrDefaultAsync(m => m.LegacyId == price.SupplierId,
                cancellationToken: cancellationToken);

            var checkUrlResult = !string.IsNullOrEmpty(price.SupplierProductCode) && CheckUrl(price.SupplierProductCode);

            var currencyRate = price.SupplierId.HasValue && _supplierCurrencyRates.ContainsKey(price.SupplierId.Value)
                ? _supplierCurrencyRates[price.SupplierId.Value]
                : null;

            return new SupplierPrice(
                price.Id,
                price.Date,
                await GetProductMapping(price.Product.Code, price.Product.NonCashProductId, cancellationToken),
                price.SupplierId.HasValue ? new IdMap(price.SupplierId.Value, supplierMap?.ErpGuid) : null,
                price.Price,
                !checkUrlResult ? price.SupplierProductCode : "",
                checkUrlResult ? price.SupplierProductCode : "",
                price.Monitor,
                price.PriceRetail,
                price.PriceDialer,
                price.Currency,
                price.IsInStock != null && !_notInStockStatuses.ContainsKey(price.IsInStock),
                PaymentTypes.Cash,
                currencyRate?.RateNal,
                price.IsInStock,
                currencyRate?.Date
            );
        }

        private async Task<SupplierPrice> MapToDomainForCashless(SupplierActualPriceEF price, CancellationToken cancellationToken)
        {
            var supplierMap = await _mapDb.ClientMaps.FirstOrDefaultAsync(m => m.LegacyId == price.SupplierId,
                cancellationToken: cancellationToken);

            var checkUrlResult = !string.IsNullOrEmpty(price.SupplierProductCode) && CheckUrl(price.SupplierProductCode);

            var currencyRate = price.SupplierId.HasValue && _supplierCurrencyRates.ContainsKey(price.SupplierId.Value)
                ? _supplierCurrencyRates[price.SupplierId.Value]
                : null;

            return new SupplierPrice(
                price.Id,
                price.Date,
                await GetProductMapping(price.Product.Code, price.Product.NonCashProductId, cancellationToken),
                price.SupplierId.HasValue ? new IdMap(price.SupplierId.Value, supplierMap?.ErpGuid) : null,
                currencyRate?.RateBn != null ? price.Price * currencyRate.RateBn : price.Price,
                !checkUrlResult ? price.SupplierProductCode : "",
                checkUrlResult ? price.SupplierProductCode : "",
                price.Monitor,
                null,
                currencyRate?.RateBn != null ? price.PriceDialer * currencyRate.RateBn : price.PriceDialer,
                price.Currency,
                price.IsInStock != null && !_notInStockStatuses.ContainsKey(price.IsInStock),
                PaymentTypes.Cashless,
                currencyRate?.RateBn,
                price.IsInStock,
                currencyRate?.Date
            );
        }

        private bool CheckUrl(string url)
        {
            var pattern = "^(http|http(s)?://)";
            var regex = new Regex(pattern);
            return regex.IsMatch(url);
        }

        private async Task<IEnumerable<SupplierPrice>> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                return new List<SupplierPrice>();
            }

            var result = new List<SupplierPrice>();
            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double) notFullMappingCount / _notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                var pricesEfByNotFullMapping = await _db.SupplierActualPrices
                    .Where(hp =>
                        notFullMappingIds.Skip(i * _notFullMappingFilterPortion).Take(_notFullMappingFilterPortion)
                            .Any(id => id == hp.Id))
                    .Join(_db.Clients, sp => sp.SupplierId, p => p.Id, (sp, c) =>

                        new SupplierActualPriceEF
                        {
                            Id = sp.Id,
                            Date = sp.Date,
                            ProductId = sp.ProductId,
                            Product = sp.Product,
                            SupplierId = sp.SupplierId,
                            Price = sp.Price,
                            PriceRetail = sp.PriceRetail,
                            PriceDialer = sp.PriceDialer,
                            SupplierProductCode = sp.SupplierProductCode,
                            Monitor = sp.Monitor,
                            IsInStock = sp.IsInStock,
                            Currency = c.BalanceCurrencyId,
                        }
                    )
                    .ToListAsync(cancellationToken: cancellationToken);

                result.AddRange((pricesEfByNotFullMapping
                    .Select(async p => await MapToDomainForCash(p, cancellationToken))
                    .Select(p => p.Result)));
            }

            return result;
        }

        private async Task<IEnumerable<int>> GetClientsIdsForFilter(CancellationToken cancellationToken)
        {
            var mappedClientsIds = await _mapDb.ClientMaps
                .Select(p => p.LegacyId).ToListAsync(cancellationToken);
            var notFullMappingClientsIds = await _mapDb.NotFullMapped
                .Where(i => i.Type == MappingTypes.Client)
                .Select(i => i.InnerId).ToListAsync(cancellationToken);
            mappedClientsIds.AddRange(notFullMappingClientsIds);

            return mappedClientsIds;
        }

        private IEnumerable<SupplierActualPriceEF> FilterNullPrices(IEnumerable<SupplierActualPriceEF> pricesEf)
        {
            return pricesEf.Where(p => p.Price != null && p.Price != 0
                                       || p.PriceRetail != null && p.PriceRetail != 0
                                       || p.PriceDialer != null && p.PriceDialer != 0);
        }

        private DateTime? GetLastDate(IEnumerable<SupplierPrice> prices)
        {
            return prices.Max(e => e.Date);
        }

        private DateTime? GetLastPackageDate(IEnumerable<SupplierPricePackage> pricePackages)
        {
            return pricePackages.SelectMany(pricePackage => pricePackage.SupplierPrices).Max(e => e.Date);
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }
    }
}
