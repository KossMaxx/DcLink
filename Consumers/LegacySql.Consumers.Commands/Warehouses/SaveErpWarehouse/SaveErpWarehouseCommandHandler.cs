using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MediatR;
using MessageBus.Warehouses.Import;

namespace LegacySql.Consumers.Commands.Warehouses.SaveErpWarehouse
{
    public class SaveErpWarehouseCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpWarehouseDto>>
    {
        private readonly IDbConnection _db;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private ExternalMap _warehouseMapping;

        public SaveErpWarehouseCommandHandler(
            IDbConnection db, 
            IWarehouseMapRepository warehouseMapRepository)
        {
            _db = db;
            _warehouseMapRepository = warehouseMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpWarehouseDto> command, CancellationToken cancellationToken)
        {
            var warehouse = command.Value;
            _warehouseMapping = await _warehouseMapRepository.GetByErpAsync(warehouse.Id);
            if (_warehouseMapping == null)
            {
                await Create(warehouse, command.MessageId);
            }
            else
            {
                await Update(warehouse);
            }

            return new Unit();
        }

        private async Task Update(ErpWarehouseDto warehouse)
        {
            var updateSqlQuery = @"update [dbo].[Склады]
                                 set [sklad_desc]=@Title
                                 where [sklad_ID]=@Id";

            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id = _warehouseMapping.LegacyId, warehouse.Title
            });

            await _warehouseMapRepository.SaveAsync(
                new WarehouseMap(
                    _warehouseMapping.MapId, 
                    _warehouseMapping.LegacyId, 
                    _warehouseMapping.ExternalMapId), 
                _warehouseMapping.Id);
        }

        private async Task Create(ErpWarehouseDto warehouse, Guid mapId)
        {
            const string insertSqlQuery = @"BEGIN TRAN
                                            DECLARE @legacy_id int
                                            UPDATE Склады SET [sklad_desc]=@Title WHERE [sklad_desc] = @Title;
                                            IF @@ROWCOUNT = 0
                                            BEGIN
                                            INSERT INTO [dbo].[Склады] ([sklad_desc]) VALUES (@Title); 
                                            SET @legacy_id = SCOPE_IDENTITY(); 
                                            END
                                            ELSE
                                            SELECT @legacy_id = sklad_ID FROM Склады WHERE [sklad_desc] = @Title;

                                            SELECT @legacy_id;
                                            COMMIT TRAN";

            var newWarehouseId = (await _db.QueryAsync<int>(insertSqlQuery, new {
                warehouse.Title
            })).FirstOrDefault();

            await _warehouseMapRepository.SaveAsync(
                new WarehouseMap(
                    mapId, 
                    newWarehouseId, 
                    warehouse.Id));
        }
    }
}
