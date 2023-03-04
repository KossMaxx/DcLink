using System;
using LegacySql.Domain.Rejects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;

namespace LegacySql.Legacy.Data.Repositories
{
    public class RejectRepository : ILegacyRejectRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;
        private readonly IProductMappingResolver _productMappingResolver;
        int[] _clientCloseStatuses = { 2, 3, 5, 6, 8, 10 };

        public RejectRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion, IProductMappingResolver productMappingResolver)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
        }

        private async IAsyncEnumerable<Reject> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var rejectEfs = await _db.Rejects
                .Include(r => r.Product)
                .Where(r => r.ChangedAt > new DateTime(2022, 1, 1))
                .Where(r => !_clientCloseStatuses.Contains(r.StatusForClient))
#if DEBUG
                .Take(1000)
#endif
                .Select(GeneralSelect())
                .ToListAsync(cancellationToken);

            foreach (var reject in rejectEfs)
            {
                yield return await MapToDomain(reject, cancellationToken);
            }
        }

        public async IAsyncEnumerable<Reject> GetChangedRejectsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (lastChangedDate.HasValue)
            {
                var rejectsEfByDate = await _db.Rejects
                    .Include(r => r.Product)
                    .Where(r => r.ChangedAt > lastChangedDate)
                    .Where(r => !_clientCloseStatuses.Contains(r.StatusForClient))
#if DEBUG
                    .Take(1000)
#endif
                    .Select(GeneralSelect())
                    .ToListAsync(cancellationToken);

                foreach (var reject in rejectsEfByDate)
                {
                    yield return await MapToDomain(reject, cancellationToken);
                }

                await foreach (var reject in GetByNotFullMapping(notFullMappingIds, cancellationToken))
                {
                    yield return reject;
                }

                yield break;
            }

            await foreach (var reject in GetAllAsync(cancellationToken))
            {
                yield return reject;
            }
        }

        public async IAsyncEnumerable<Reject> GetOpenRejectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            int[] clientCloseStatuses = {2, 3, 4, 5, 8, 10};
            int[] serviceCloseStatuses = {2, 3, 4, 5, 13};
            var rejectEfs = await _db.Rejects
                .Include(r => r.Product)
                .Where(r => !(clientCloseStatuses.Contains(r.StatusForClient) && serviceCloseStatuses.Contains(r.StatusForService)))
#if DEBUG
                .Take(1000)
#endif
                .Select(GeneralSelect())
                .ToListAsync(cancellationToken);

            foreach (var reject in rejectEfs)
            {
                yield return await MapToDomain(reject, cancellationToken);
            }
        }

        public async Task<Reject> GetRejectAsync(int id, CancellationToken cancellationToken)
        {
            var rejectEf = await _db.Rejects
                .Include(r => r.Product)
                .Where(r => !_clientCloseStatuses.Contains(r.StatusForClient))
                .Select(GeneralSelect())
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (rejectEf == null)
            {
                return null;
            }

            return await MapToDomain(rejectEf, cancellationToken);
        }

        private async Task<Reject> MapToDomain(RejectEF rejectEf, CancellationToken cancellationToken)
        {
            var rejectMap = await _mapDb.RejectMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.Id, cancellationToken);
            var clientMap = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.ClientId, cancellationToken);
            var warehouseMap = await _mapDb.WarehouseMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.WarehouseId, cancellationToken);
            var clientOrderMap = await _mapDb.ClientOrderMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.ClientOrderId, cancellationToken);
            var supplierMap = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.SupplierId, cancellationToken);
            var productTypeMap = await _mapDb.ProductTypeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.ProductTypeId, cancellationToken);
            var productRefundMap = await _mapDb.ProductRefundMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == rejectEf.ProductRefundId, cancellationToken);

            return new Reject
            (
                rejectMap != null,
                new IdMap(rejectEf.Id, rejectMap?.ErpGuid),
                rejectEf.CreatedAt,
                rejectEf.Date,
                rejectEf.SerialNumber,
                rejectEf.ClientTitle,
                rejectEf.ClientId.HasValue && rejectEf.ClientId > 0 
                    ? new IdMap(rejectEf.ClientId.Value, clientMap?.ErpGuid) 
                    : null,
                rejectEf.StatusForClient,
                new IdMap(rejectEf.WarehouseId, warehouseMap?.ErpGuid),
                rejectEf.ResponsibleForStatus,
                rejectEf.RepairType,
                rejectEf.DefectDescription,
                rejectEf.KitDescription,
                rejectEf.ProductStatusDescription,
                rejectEf.Notes,
                rejectEf.ProductStatus,
                rejectEf.ClientOrderId.HasValue && rejectEf.ClientOrderId > 0 ? new IdMap(rejectEf.ClientOrderId.Value, clientOrderMap?.ErpGuid) : null,
                rejectEf.ClientOrderDate,
                rejectEf.ReceiptDocumentDate,
                rejectEf.ReceiptDocumentId,
                rejectEf.SupplierId.HasValue && rejectEf.SupplierId > 0 ? new IdMap(rejectEf.SupplierId.Value, supplierMap?.ErpGuid) : null,
                rejectEf.SupplierTitle,
                rejectEf.PurchasePrice,
                rejectEf.ProductMark,
                await GetProductMapping(rejectEf.ProductId, rejectEf.Product?.NonCashProductId, cancellationToken),
                rejectEf.ProductTypeId.HasValue && rejectEf.ProductTypeId > 0 
                    ? new IdMap(rejectEf.ProductTypeId.Value, productTypeMap?.ErpGuid) 
                    : null,
                rejectEf.PurchaseCurrencyPrice,
                rejectEf.OutgoingWarranty,
                rejectEf.DepartureDate,
                rejectEf.ChangedAt,
                rejectEf.Amount,
                rejectEf.ProductRefundId.HasValue && rejectEf.ProductRefundId > 0
                    ? new IdMap(rejectEf.ProductRefundId.Value, productRefundMap?.ErpGuid)
                    : null,
                rejectEf.BuyDocDate,
                rejectEf.SellDocDate,
                rejectEf.SupplierDescription,
                rejectEf.ReturnDate,
                rejectEf.SupplierProductMark,
                rejectEf.SupplierProductId.HasValue 
                    ? await GetProductMapping(rejectEf.SupplierProductId.Value, rejectEf.SupplierProduct?.NonCashProductId, cancellationToken)
                    : null,
                rejectEf.SupplierSerialNumber,
                rejectEf.StatusForService
            );
        }

        private async IAsyncEnumerable<Reject> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = Math.Ceiling((double) notFullMappingCount / _notFullMappingFilterPortion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var rejectsEf = await _db.Rejects
                    .Include(r => r.Product)
                    .Where(p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id))
#if DEBUG
                    .Take(1000)
#endif
                    .Select(GeneralSelect())
                    .ToListAsync(cancellationToken);

                foreach (var reject in rejectsEf)
                {
                    yield return await MapToDomain(reject, cancellationToken);
                }
            }
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private Expression<Func<RejectEF,RejectEF>> GeneralSelect()
        {
            return r => new RejectEF
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                Date = r.Date,
                SerialNumber = r.SerialNumber,
                ClientTitle = r.ClientTitle,
                ClientId = r.ClientId,
                StatusForClient = r.StatusForClient,
                WarehouseId = r.WarehouseId,
                ResponsibleForStatus = r.ResponsibleForStatus,
                RepairType = r.RepairType,
                DefectDescription = r.DefectDescription,
                KitDescription = r.KitDescription,
                ProductStatusDescription = r.ProductStatusDescription,
                Notes = r.Notes,
                ProductStatus = r.ProductStatus,
                ClientOrderId = r.ClientOrderId,
                ClientOrderDate = r.ClientOrderDate,
                ReceiptDocumentDate = r.ReceiptDocumentDate,
                ReceiptDocumentId = r.ReceiptDocumentId,
                SupplierId = r.SupplierId,
                SupplierTitle = r.SupplierTitle,
                PurchasePrice = r.PurchasePrice,
                ProductMark = r.ProductMark,
                ProductId = r.ProductId,
                ProductTypeId = r.ProductTypeId,
                PurchaseCurrencyPrice = r.PurchaseCurrencyPrice,
                OutgoingWarranty = r.OutgoingWarranty,
                DepartureDate = r.DepartureDate,
                ChangedAt = r.ChangedAt,
                StatusForService = r.StatusForService,
                ProductRefundId = r.ProductRefundId,
                SupplierDescription = r.SupplierDescription,
                ReturnDate = r.ReturnDate,
                SupplierProductMark = r.SupplierProductMark,
                SupplierProductId = r.SupplierProductId,
                SupplierSerialNumber = r.SupplierSerialNumber,
                
                BuyDocDate = r.ProductRefund == null ? null : (DateTime?) r.ProductRefund.Date,
                SellDocDate = r.ClientOrder == null ? null : r.ClientOrder.Date,
            };
        }
    }
}