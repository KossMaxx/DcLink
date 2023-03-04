using System;
using LegacySql.Domain.ReconciliationActs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ReconciliationActRepository : ILegacyReconciliationActRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;

        public ReconciliationActRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
        }

        private async IAsyncEnumerable<ReconciliationAct> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var allEfs = await _db.ReconciliationActs
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            foreach (var ef in allEfs)
            {
                yield return await MapToDomain(ef, cancellationToken);
            }
        }

        public async IAsyncEnumerable<ReconciliationAct> GetChangedReconciliationActsAsync(DateTime? lastChangedDate, List<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (lastChangedDate.HasValue)
            {
                var allEfsByDate = await _db.ReconciliationActs
                    .Where(r => r.ChangedAt > lastChangedDate)
#if DEBUG
                    .Take(1000)
#endif
                    .ToListAsync(cancellationToken);

                foreach (var ef in allEfsByDate)
                {
                    yield return await MapToDomain(ef, cancellationToken);
                }

                await foreach (var ef in GetByNotFullMapping(notFullMappingIds, cancellationToken))
                {
                    yield return ef;
                }

                yield break;
            }

            await foreach (var ef in GetAllAsync(cancellationToken))
            {
                yield return ef;
            }
        }

        public async Task<ReconciliationAct> GetReconciliationActAsync(int id, CancellationToken cancellationToken)
        {
            var ef = await _db.ReconciliationActs
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (ef == null)
            {
                return null;
            }

            return await MapToDomain(ef, cancellationToken);
        }

        private async Task<ReconciliationAct> MapToDomain(ReconciliationActEF reconciliationActEf, CancellationToken cancellationToken)
        {
            var reconciliationActMap = await _mapDb.ReconciliationActMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == reconciliationActEf.Id, cancellationToken);
            var clientMap = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == reconciliationActEf.ClientId, cancellationToken);

            return new ReconciliationAct
            (
                reconciliationActMap != null,
                new IdMap(reconciliationActEf.Id, reconciliationActMap?.ErpGuid),
                reconciliationActEf.Sum,
                reconciliationActEf.ChangedAt,
                reconciliationActEf.ClientId.HasValue && reconciliationActEf.ClientId > 0 ? new IdMap(reconciliationActEf.ClientId.Value, clientMap?.ErpGuid) : null,
                reconciliationActEf.IsApproved
            );
        }

        private async IAsyncEnumerable<ReconciliationAct> GetByNotFullMapping(List<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count;
            var cycleLimitation = Math.Ceiling((double) notFullMappingCount / _notFullMappingFilterPortion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var reconciliationActsEf = await _db.ReconciliationActs
                    .Where(p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id))
#if DEBUG
                    .Take(1000)
#endif
                    .ToListAsync(cancellationToken);

                foreach (var reconciliationAct in reconciliationActsEf)
                {
                    yield return await MapToDomain(reconciliationAct, cancellationToken);
                }
            }
        }
    }
}