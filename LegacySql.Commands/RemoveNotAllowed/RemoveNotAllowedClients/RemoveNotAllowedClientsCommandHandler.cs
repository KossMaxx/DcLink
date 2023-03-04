using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Legacy.Data;
using MassTransit;
using MediatR;
using MessageBus.Clients.Export.NotAllowedClients;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Commands.RemoveNotAllowed.RemoveNotAllowedClients
{
    public class RemoveNotAllowedClientsCommandHandler : IRequestHandler<RemoveNotAllowedClientsCommand>
    {
        private readonly List<int> NotAllowedDepartmentIds = new List<int> {34, 43};
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly IBus _bus;

        public RemoveNotAllowedClientsCommandHandler(LegacyDbContext db, AppDbContext mapDb, IBus bus)
        {
            _db = db;
            _mapDb = mapDb;
            _bus = bus;
        }

        public async Task<Unit> Handle(RemoveNotAllowedClientsCommand command,
            CancellationToken cancellationToken)
        {
            var notAllowedClientsIds = await GetNotAllowedClientsIds(cancellationToken);
            var mappingsOfNotAllowedClients = await _mapDb.ClientMaps
                .Where(c => notAllowedClientsIds.Contains(c.LegacyId)).ToListAsync(cancellationToken);
            var erpGuids = mappingsOfNotAllowedClients.Where(m => m.ErpGuid.HasValue).Select(m => m.ErpGuid);

            await WriteGuidToFile(erpGuids);
            await RemoveNotAllowedMappings(mappingsOfNotAllowedClients, cancellationToken);

            return new Unit();
        }

        private async Task RemoveNotAllowedMappings(IEnumerable<ClientMapEF> mappingsOfNotAllowedClients, CancellationToken cancellationToken)
        {
            foreach (var mapping in mappingsOfNotAllowedClients)
            {
                await _bus.Publish(new NotAllowedClientMessage {MessageId = mapping.MapGuid}, cancellationToken);

                var notAllowedClientMap =
                    await _mapDb.ClientMaps.FirstOrDefaultAsync(m => m.LegacyId == mapping.LegacyId, cancellationToken);
                _mapDb.Remove(notAllowedClientMap);
                await _mapDb.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task WriteGuidToFile(IEnumerable<Guid?> erpGuids)
        {
            await using (var sw = new StreamWriter(
                $"{Path.GetFullPath($"ErpGuidsOfNotAllowedClients_{DateTime.Now:MM_dd_yyyy}.txt")}", 
                false, 
                System.Text.Encoding.Default))
            {
                foreach (var guid in erpGuids)
                {
                    sw.WriteLine(guid);
                }
            }
        }

        private async Task<IEnumerable<long>> GetNotAllowedClientsIds(CancellationToken cancellationToken)
        {
            var notAllowedClientsIds = await _db.Clients
                .Where(c => c.Department.HasValue && NotAllowedDepartmentIds.Contains(c.Department.Value) || c.IsTechnicalAccount)
                .Select(c => (long)c.Id).ToListAsync(cancellationToken);

            return notAllowedClientsIds;
        }
    }
}
