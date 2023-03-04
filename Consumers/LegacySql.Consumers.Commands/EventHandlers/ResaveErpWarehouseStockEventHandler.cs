using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.WarehouseStocks;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpWarehouseStockEventHandler : INotificationHandler<ResaveErpWarehouseStockEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpWarehouseStockSaver _warehouseStockSaver;

        public ResaveErpWarehouseStockEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpWarehouseStockSaver warehouseStockSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _warehouseStockSaver = warehouseStockSaver;
        }

        public async Task Handle(ResaveErpWarehouseStockEvent notification, CancellationToken cancellationToken)
        {
            foreach (var stock in notification.Messages)
            {
                _warehouseStockSaver.InitErpObject(stock);

                var mapInfo = await _warehouseStockSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                   continue;
                }

                await _warehouseStockSaver.SaveErpObject();

                await _erpNotFullMappedRepository.RemoveAsync(stock.Id, MappingTypes.WarehouseStock);
            }
        }
    }
}
