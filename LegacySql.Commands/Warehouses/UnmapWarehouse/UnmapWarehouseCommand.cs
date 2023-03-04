using System;
using MediatR;

namespace LegacySql.Commands.Warehouses.UnmapWarehouse
{
    public class UnmapWarehouseCommand : IRequest
    {
        public UnmapWarehouseCommand(Guid erpId)
        {
            ErpId = erpId;
        }

        public Guid ErpId { get; }
    }
}
