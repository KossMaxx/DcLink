using LegacySql.Consumers.Commands.BankPayments;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpPaymentOrderEventHandler : INotificationHandler<ResaveErpPaymentOrderEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IPaymentOrderMapRepository _paymentOrderMapRepository;
        private readonly ErpPaymentOrderSaver _erpPaymentOrderSaver;

        public ResaveErpPaymentOrderEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository,
            IPaymentOrderMapRepository paymentOrderMapRepository,
            ErpPaymentOrderSaver erpPaymentOrderSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _paymentOrderMapRepository = paymentOrderMapRepository;
            _erpPaymentOrderSaver = erpPaymentOrderSaver;
        }

        public async Task Handle(ResaveErpPaymentOrderEvent notification, CancellationToken cancellationToken)
        {
            foreach (var payment in notification.Messages)
            {
                var paymentMapping = await _paymentOrderMapRepository.GetByErpAsync(payment.Id);
                _erpPaymentOrderSaver.InitErpObject(payment, paymentMapping);

                var mappingInfo = await _erpPaymentOrderSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (paymentMapping == null)
                {
                    await _erpPaymentOrderSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpPaymentOrderSaver.Update();
                }
                await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.PaymentOrder);
            }   
        }
    }
}
