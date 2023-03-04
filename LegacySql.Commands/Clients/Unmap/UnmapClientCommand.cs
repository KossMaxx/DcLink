using System;
using MediatR;

namespace LegacySql.Commands.Clients.Unmap
{
    public class UnmapClientCommand : IRequest
    {
        public UnmapClientCommand(Guid erpId)
        {
            ErpId = erpId;
        }

        public Guid ErpId { get; }
    }
}
