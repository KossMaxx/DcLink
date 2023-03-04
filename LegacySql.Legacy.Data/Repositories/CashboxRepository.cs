using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class CashboxRepository : ILegacyCashboxRepository
    {
        private LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public CashboxRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }
 

        public async Task<Cashbox> Get(int id, CancellationToken cancellationToken)
        {
            var сashboxEf = await _db.Cashboxes
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return сashboxEf == null ? null : await MapToDomain(сashboxEf, cancellationToken);
        }

        private async Task<Cashbox> MapToDomain(CashboxEF сashboxEf, CancellationToken cancellationToken)
        {
            var сashboxMap = await _mapDb.CashboxMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == сashboxEf.Id, cancellationToken: cancellationToken);
            return new Cashbox(
                new IdMap(сashboxEf.Id, сashboxMap?.ErpGuid),
                сashboxEf.Description
            );
        }
    }
}