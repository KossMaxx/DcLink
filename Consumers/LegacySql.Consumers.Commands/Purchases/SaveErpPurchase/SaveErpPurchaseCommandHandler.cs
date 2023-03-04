using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Purchases.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Purchases.SaveErpPurchase
{
    public class SaveErpProductReceiptReceiptCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpPurchaseDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpPurchaseSaver _erpPurchaseSaver;
        private readonly IPurchaseMapRepository _purchaseMapRepository;

        public SaveErpProductReceiptReceiptCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpPurchaseSaver erpPurchaseSaver, 
            IPurchaseMapRepository purchaseMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpPurchaseSaver = erpPurchaseSaver;
            _purchaseMapRepository = purchaseMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpPurchaseDto> command, CancellationToken cancellationToken)
        {
            var purchase = command.Value;

            var purchaseMapping = await _purchaseMapRepository.GetByErpAsync(purchase.Id);
            _erpPurchaseSaver.InitErpObject(purchase, purchaseMapping);
            
            var mapInfo = await _erpPurchaseSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(purchase, mapInfo.Why);
                return new Unit();
            }

            await _erpPurchaseSaver.SaveErpObject(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(purchase.Id, MappingTypes.Purchase);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpPurchaseDto purchase, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                purchase.Id,
                MappingTypes.Purchase,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(purchase)
            ));
        }
    }
}