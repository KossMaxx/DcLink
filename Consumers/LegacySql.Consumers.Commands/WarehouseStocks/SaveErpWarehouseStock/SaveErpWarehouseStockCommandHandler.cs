using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.WarehouseStocks.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.WarehouseStocks.SaveErpWarehouseStock
{
    public class SaveErpWarehouseStockCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpWarehouseStockDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpWarehouseStockSaver _warehouseStockSaver;

        public SaveErpWarehouseStockCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpWarehouseStockSaver warehouseStockSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _warehouseStockSaver = warehouseStockSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpWarehouseStockDto> command, CancellationToken cancellationToken)
        {
            var stock = command.Value;
            _warehouseStockSaver.InitErpObject(stock);

            var mapInfo = await _warehouseStockSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(stock, mapInfo.Why);
                return new Unit();
            }

            await _warehouseStockSaver.SaveErpObject();
            await _erpNotFullMappedRepository.RemoveAsync(stock.Id, MappingTypes.WarehouseStock);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpWarehouseStockDto stock, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                stock.Id, 
                MappingTypes.WarehouseStock,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(stock)
            ));
        }
    }
}
