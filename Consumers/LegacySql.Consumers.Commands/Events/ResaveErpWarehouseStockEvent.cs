using System.Collections.Generic;
using MediatR;
using MessageBus.WarehouseStocks.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpWarehouseStockEvent : INotification
    {
        public ResaveErpWarehouseStockEvent(List<ErpWarehouseStockDto> messages)
        {
            Messages = messages;
        }

        public List<ErpWarehouseStockDto> Messages { get; }
    }
}
