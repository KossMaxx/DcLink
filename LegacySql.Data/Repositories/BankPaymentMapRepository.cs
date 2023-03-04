using LegacySql.Data.Models;
using LegacySql.Domain.BankPayments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegacySql.Data.Repositories
{
    public class BankPaymentMapRepository : BaseMapRepository<BankPaymentMapEF>, IBankPaymentMapRepository
    {
        public BankPaymentMapRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<BankPaymentMap>> GetRangeByErpIdAsync(Guid id)
        {
            var mapsEF = await _db.BankPaymentMaps.Where(e => e.ErpGuid == id).ToListAsync();
            return mapsEF.Select(e=> new BankPaymentMap(e.MapGuid, e.LegacyId,e.ClientOrderId,e.ErpGuid, e.Id));
        }

        public async Task SaveAsync(BankPaymentMap newMap, Guid? id = null)
        {
            BankPaymentMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.BankPaymentMaps.FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyId = newMap.LegacyId;
                mapEf.ErpGuid = newMap.ExternalMapId;
                mapEf.ClientOrderId = newMap.ClientOrderId;
            }
            else
            {
                mapEf = await _db.BankPaymentMaps
                    .FirstOrDefaultAsync(m => m.ClientOrderId.HasValue && m.LegacyId == newMap.LegacyId && m.ClientOrderId == newMap.ClientOrderId);
                if (mapEf == null)
                {
                    mapEf = new BankPaymentMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        ErpGuid = newMap.ExternalMapId,
                        ClientOrderId = newMap.ClientOrderId
                };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }
    }
}
