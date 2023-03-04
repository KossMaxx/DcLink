using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Purchases;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpPurchaseCommandHandler : INotificationHandler<ResaveErpPurchaseEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpPurchaseSaver _erpPurchaseSaver;
        private readonly IPurchaseMapRepository _purchaseMapRepository;

        public ResaveErpPurchaseCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, ErpPurchaseSaver _erpPurchaseSaver, IPurchaseMapRepository purchaseMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            this._erpPurchaseSaver = _erpPurchaseSaver;
            _purchaseMapRepository = purchaseMapRepository;
        }

        public async Task Handle(ResaveErpPurchaseEvent notification, CancellationToken cancellationToken)
        {
            foreach (var purchase in notification.Messages)
            {
                var purchaseMapping = await _purchaseMapRepository.GetByErpAsync(purchase.Id);
                _erpPurchaseSaver.InitErpObject(purchase, purchaseMapping);

                var mapInfo = await _erpPurchaseSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpPurchaseSaver.SaveErpObject(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(purchase.Id, MappingTypes.Purchase);
            }
        }
    }
}