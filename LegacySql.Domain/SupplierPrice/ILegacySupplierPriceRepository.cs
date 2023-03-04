using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.SupplierPrice
{
    public interface ILegacySupplierPriceRepository
    {
        Task<(IEnumerable<SupplierPrice> supplierPrice, DateTime? lastDate)> GetChangedSupplierPricesAsync(IEnumerable<int> filteredProductsIds, DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<(IEnumerable<SupplierPricePackage> supplierPrice, DateTime? lastDate)> GetChangedSupplierPricePackagesAsync(IEnumerable<int> filteredProductsIds, DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<IEnumerable<SupplierPrice>> GetAllAsync(IEnumerable<int> filteredProductsIds, DateTime? date, CancellationToken cancellationToken);
        Task<IEnumerable<SupplierPricePackage>> GetAllPackagesAsync(IEnumerable<int> filteredProductsIds, DateTime? date, CancellationToken cancellationToken);
        Task<IEnumerable<SupplierPrice>> GetInitialSellingPricesAsync(IEnumerable<int> filteredProductsIds, CancellationToken cancellationToken);
        Task<IEnumerable<SupplierPricePackage>> GetInitialSellingPricePackagesAsync(IEnumerable<int> filteredProductsIds, CancellationToken cancellationToken);
        
    }
}
