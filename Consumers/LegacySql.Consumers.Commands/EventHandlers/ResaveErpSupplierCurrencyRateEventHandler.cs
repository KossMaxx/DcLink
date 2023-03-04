using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.SupplierCurrencyRates;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpSupplierCurrencyRateEventHandler : INotificationHandler<ResaveErpSupplierCurrencyRateEvent>
    {
        private ErpSupplierCurrencyRateSaver _erpSupplierCurrencyRateSaver;

        public ResaveErpSupplierCurrencyRateEventHandler(
            ErpSupplierCurrencyRateSaver erpSupplierCurrencyRateSaver)
        {
            _erpSupplierCurrencyRateSaver = erpSupplierCurrencyRateSaver;
        }

        public async Task Handle(ResaveErpSupplierCurrencyRateEvent notification, CancellationToken cancellationToken)
        {
            foreach (var rate in notification.Messages)
            {
                _erpSupplierCurrencyRateSaver.InitErpObject(rate);

                var mapInfo = await _erpSupplierCurrencyRateSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpSupplierCurrencyRateSaver.SaveErpObject(Guid.NewGuid());
            }
        }
    }
}
