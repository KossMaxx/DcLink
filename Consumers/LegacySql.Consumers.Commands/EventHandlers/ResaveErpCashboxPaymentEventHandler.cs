using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Cashboxes;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpCashboxPaymentEventHandler : INotificationHandler<ResaveErpCashboxPaymentEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ICashboxPaymentMapRepository _cashboxPaymentMapRepository;
        private readonly ErpCashboxPaymentSaver _erpCashboxPaymentSaver;

        public ResaveErpCashboxPaymentEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ICashboxPaymentMapRepository cashboxPaymentMapRepository, 
            ErpCashboxPaymentSaver erpCashboxPaymentSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _cashboxPaymentMapRepository = cashboxPaymentMapRepository;
            _erpCashboxPaymentSaver = erpCashboxPaymentSaver;
        }

        public async Task Handle(ResaveErpCashboxPaymentEvent notification, CancellationToken cancellationToken)
        {
            foreach (var payment in notification.Messages)
            {
                var paymentMapping = await _cashboxPaymentMapRepository.GetByErpAsync(payment.Id);
                _erpCashboxPaymentSaver.InitErpObject(payment, paymentMapping);

                var mappingInfo = await _erpCashboxPaymentSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (paymentMapping == null)
                {
                    await _erpCashboxPaymentSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpCashboxPaymentSaver.Update();
                }

                await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.CashboxPayment);
            }
        }
    }
}
