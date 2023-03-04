using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductTypes;
using LegacySql.Legacy.Data;
using MassTransit;
using MediatR;
using MessageBus.Products.Export.NotAllowedProducts;
using MessageBus.ProductTypes.Export.NotAllowedProductTypes;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Commands.RemoveNotAllowed.RemoveNotUsedProductType
{
    public class RemoveNotUsedProductTypeCommandHandler: IRequestHandler<RemoveNotUsedProductTypeCommand>
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly IBus _bus;
        private List<int> _notUsedTypesIds;

        public RemoveNotUsedProductTypeCommandHandler(LegacyDbContext db, AppDbContext mapDb, IBus bus, ILegacyProductTypeRepository legacyProductTypeRepository)
        {
            _db = db;
            _mapDb = mapDb;
            _bus = bus;
            _notUsedTypesIds = legacyProductTypeRepository.GetNotUsedTypesIds();
        }

        public async Task<Unit> Handle(RemoveNotUsedProductTypeCommand command, CancellationToken cancellationToken)
        {
            var mappingsOfNotAllowedProductTypes = await _mapDb.ProductTypeMaps
                .Where(t => _notUsedTypesIds.Any(id=> id == t.LegacyId)).ToListAsync(cancellationToken);
            var productTypesErpGuids = mappingsOfNotAllowedProductTypes.Where(m => m.ErpGuid.HasValue).Select(m => m.ErpGuid);
            await WriteToFile($"ErpGuidsOfNotAllowedProductTypes_{DateTime.Now:MM_dd_yyyy}.txt", productTypesErpGuids);
            
            foreach (var mapping in mappingsOfNotAllowedProductTypes)
            {
                await _bus.Publish(new NotAllowedProductTypeMessage { MessageId = mapping.MapGuid }, cancellationToken);

                var notAllowedProductMap = await _mapDb.ProductTypeMaps.FirstOrDefaultAsync(m => m.LegacyId == mapping.LegacyId, cancellationToken);
                _mapDb.Remove(notAllowedProductMap);
                await _mapDb.SaveChangesAsync(cancellationToken);
            }

            var notAllowedProductsIds = await _db.Products
                .Where( p=> _notUsedTypesIds.Any(t=> t == (long)p.ProductTypeId))
                .Select(p => (long)p.Code)
                .ToListAsync(cancellationToken);
            var mappingsOfNotAllowedProducts = await _mapDb.ProductMaps
                .Where(p => notAllowedProductsIds.Any(id => id == p.LegacyId)).ToListAsync(cancellationToken);
            var productsErpGuids = mappingsOfNotAllowedProducts.Where(m => m.ErpGuid.HasValue).Select(m => m.ErpGuid);

            await WriteToFile($"ErpGuidsOfNotAllowedProductsByTypes_{DateTime.Now:MM_dd_yyyy}.txt", productsErpGuids);
            
            foreach (var mapping in mappingsOfNotAllowedProducts)
            {
                await _bus.Publish(new NotAllowedProductMessage { MessageId = mapping.MapGuid }, cancellationToken);

                var notAllowedProductMap = await _mapDb.ProductMaps.FirstOrDefaultAsync(m => m.LegacyId == mapping.LegacyId, cancellationToken);
                _mapDb.Remove(notAllowedProductMap);
                await _mapDb.SaveChangesAsync(cancellationToken);
            }

            return new Unit();
        }

        private async Task WriteToFile(string fileName, IEnumerable<Guid?> guids)
        {
            await using (StreamWriter sw = new StreamWriter($"{Path.GetFullPath(fileName)}", false, System.Text.Encoding.Default))
            {
                foreach (var guid in guids)
                {
                    sw.WriteLine(guid);
                }
            }
        }
    }
}
