using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Warehouses;
using MediatR;

namespace LegacySql.Commands.Warehouses.MapWarehouse
{
    public class MapWarehouseCommandHandler : IRequestHandler<MapWarehouseCommand>
    {
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly ILegacyWarehouseRepository _legacyWarehouseRepository;

        public MapWarehouseCommandHandler(IWarehouseMapRepository warehouseMapRepository, ILegacyWarehouseRepository legacyWarehouseRepository)
        {
            _warehouseMapRepository = warehouseMapRepository;
            _legacyWarehouseRepository = legacyWarehouseRepository;
        }

        public async Task<Unit> Handle(MapWarehouseCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _warehouseMapRepository.GetByLegacyAsync(command.InnerId);

            if (mapping == null)
            {
                var legacyWarehouse = await _legacyWarehouseRepository.Get(command.InnerId, cancellationToken);
                if (legacyWarehouse == null)
                {
                    throw new KeyNotFoundException($"Склад с Id: {command.InnerId} не найден");
                }

                mapping = new WarehouseMap(Guid.NewGuid(), command.InnerId, command.ExternalId);
                await _warehouseMapRepository.SaveAsync(mapping);
            }
            else
            {
                mapping.MapToExternalId(command.ExternalId);
                await _warehouseMapRepository.SaveAsync(mapping, mapping.Id);
            }
            
            return new Unit();
        }
    }
}
