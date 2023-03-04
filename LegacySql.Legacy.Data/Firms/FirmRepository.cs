using LegacySql.Data;
using LegacySql.Domain.Firms;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Firms
{
    public class FirmRepository : ILegacyFirmRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;

        public FirmRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
        }
        private async IAsyncEnumerable<Firm> GetAllAsync(Expression<Func<FirmEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var firmsQuery = _db.Firms
                .FromSqlRaw($@"select * from [dbo].[Firms]")
                .Include(e => e.Client)
                .AsQueryable();

            if (filter != null)
            {
                firmsQuery = firmsQuery.Where(filter);
            }
//#if DEBUG
//            firmsQuery = firmsQuery.Take(1000);
//#endif
            var firms = await firmsQuery.ToListAsync(cancellationToken);

            await foreach (var firm in MapToDomain(firms, cancellationToken))
            {
                yield return firm;
            }
        }
        public async IAsyncEnumerable<Firm> GetChangedFirmsAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<FirmEF, bool>> filter = PredicateBuilder.New<FirmEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.LastChangeDate > changedAt);
            }

            await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
            {
                yield return mappedItem;
            }

            await foreach (var mappedItem in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        public async Task<Firm> GetFirmAsync(int id, CancellationToken cancellationToken)
        {
            var firmEF = await _db.Firms
                .Include(e => e.Client)
                .Where(e=>e.Id == id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (firmEF == null)
            {
                return null;
            }

            return await MapToDomain(firmEF, cancellationToken);
        }

        private async IAsyncEnumerable<Firm> MapToDomain(IEnumerable<FirmEF> firmsEf, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var p in firmsEf)
            {
                var firm = await MapToDomain(p, cancellationToken);
                if (firm != null)
                {
                    yield return firm;
                }
            }
        }

        private async Task<Firm> MapToDomain(FirmEF firmEF, CancellationToken cancellationToken)
        {
            var isClientExist = firmEF.ClientId.HasValue && firmEF.ClientId > 0 && firmEF.Client != null;
            var firmMap = await _mapDb.FirmMaps.AsNoTracking().
                FirstOrDefaultAsync(b => b.LegacyId == firmEF.Id, cancellationToken);
            var clientMap = isClientExist
                ? await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == firmEF.ClientId, cancellationToken: cancellationToken)
                : null;
            var masterClientMap = isClientExist
                ? await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == firmEF.Client.MasterId, cancellationToken: cancellationToken)
                : null;

            return new Firm(id: new IdMap(firmEF.Id, firmMap?.ErpGuid),
            taxCode: firmEF.TaxCode,
            title: firmEF.Title,
            legalAddress: firmEF.LegalAddress,
            address: firmEF.Address,
            phone: firmEF.Phone,
            account: firmEF.Account,
            bankCode: firmEF.BankCode,
            bankName: firmEF.BankName,
            payerCode: firmEF.PayerCode,
            certificateNumber: firmEF.CertificateNumber,
            notVat: firmEF.NotVat,
            changedAt: firmEF.LastChangeDate,
            clientId: isClientExist ? new IdMap(firmEF.ClientId.Value, clientMap?.ErpGuid) : null,
            hasMap: firmMap != null,
            masterClientId: isClientExist ? new IdMap(firmEF.Client.MasterId, masterClientMap?.ErpGuid) : null,
            isNotResident: firmEF.IsNotResident
            );
        }

        private async IAsyncEnumerable<Firm> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double)notFullMappingCount / _notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                Expression<Func<FirmEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }
    }
}
