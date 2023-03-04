using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.BankPayments;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpBankPaymentEventHandler : INotificationHandler<ResaveErpBankPaymentEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IBankPaymentMapRepository _bankPaymentMapRepository;
        private readonly ErpBankPaymentSaver _erpBankPaymentSaver;

        public ResaveErpBankPaymentEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IBankPaymentMapRepository bankPaymentMapRepository, 
            ErpBankPaymentSaver erpBankPaymentSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _bankPaymentMapRepository = bankPaymentMapRepository;
            _erpBankPaymentSaver = erpBankPaymentSaver;
        }

        public async Task Handle(ResaveErpBankPaymentEvent notification, CancellationToken cancellationToken)
        {
            foreach (var payment in notification.Messages)
            {
                MappingInfo mappingInfo = null;
                foreach (var clientOrder in payment.ClientOrders)
                {
                    mappingInfo = await _erpBankPaymentSaver.GetMappingInfo(clientOrder.ClientId);
                    if (!mappingInfo.IsMappingFull)
                    {
                        break;
                    }
                }

                if (mappingInfo != null && !mappingInfo.IsMappingFull)
                {
                    continue;
                }

                var paymentMappings = await _bankPaymentMapRepository.GetRangeByErpIdAsync(payment.Id);

                var paymentIdsWithoutClientOrder = paymentMappings.Where(e => !e.ClientOrderId.HasValue).Select(e => e.LegacyId);
                await _erpBankPaymentSaver.DeletePaymentOrderWithoutClientOrder(paymentIdsWithoutClientOrder);

                var currentClientOrderIds = payment.ClientOrders.Where(e => e.ClientOrderId.HasValue).Select(e => (int)e.ClientOrderId);
                var clientOrderIdsWithPayment = paymentMappings.Where(e => e.ClientOrderId.HasValue).Select(e => (int)e.ClientOrderId);

                var clientOrderIdsForDelete = clientOrderIdsWithPayment.Except(currentClientOrderIds);
                var clientOrderIdsForUpdate = clientOrderIdsWithPayment.Intersect(currentClientOrderIds);
                var clientOrderIdsForCreate = currentClientOrderIds.Except(clientOrderIdsWithPayment);
                var paymentClientOrdersForCreate = payment.ClientOrders.Where(e => !e.ClientOrderId.HasValue || clientOrderIdsForCreate.Any(c => c == e.ClientOrderId));

                foreach (var clientOrderId in clientOrderIdsForDelete)
                {
                    var paymentMapping = paymentMappings.First(e => e.ClientOrderId == clientOrderId);
                    _erpBankPaymentSaver.InitErpObject(payment, paymentMapping, null);
                    await _erpBankPaymentSaver.DeletePaymentOrder();
                }

                foreach (var clientOrderId in clientOrderIdsForUpdate)
                {
                    var paymentMapping = paymentMappings.First(e => e.ClientOrderId == clientOrderId);
                    _erpBankPaymentSaver.InitErpObject(payment, paymentMapping, payment.ClientOrders.First(e => e.ClientOrderId == clientOrderId));
                    await _erpBankPaymentSaver.Update();
                }

                foreach (var paymentClientOrder in paymentClientOrdersForCreate)
                {
                    _erpBankPaymentSaver.InitErpObject(payment, null, paymentClientOrder);
                    await _erpBankPaymentSaver.Create(Guid.NewGuid());
                }

                await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.BankPayment);
            }
        }
    }
}
