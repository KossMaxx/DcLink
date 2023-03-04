using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Warehouses;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class WarehouseRepository : ILegacyWarehouseRepository
    {
        private LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public WarehouseRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }
 

        public async Task<Warehouse> Get(int id, CancellationToken cancellationToken)
        {
            var сashboxEf = await _db.Warehouses
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return сashboxEf == null ? null : await MapToDomain(сashboxEf, cancellationToken);
        }

        private async Task<Warehouse> MapToDomain(WarehouseEF сashboxEf, CancellationToken cancellationToken)
        {
            var сashboxMap = await _mapDb.WarehouseMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == сashboxEf.Id, cancellationToken: cancellationToken);
            return new Warehouse(
                new IdMap(сashboxEf.Id, сashboxMap?.ErpGuid),
                сashboxEf.Description
            );
        }
    }
}