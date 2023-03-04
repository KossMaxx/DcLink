using System;
using MediatR;

namespace LegacySql.Commands.Warehouses.MapWarehouse
{
    public class MapWarehouseCommand : IRequest
    {
        public MapWarehouseCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
