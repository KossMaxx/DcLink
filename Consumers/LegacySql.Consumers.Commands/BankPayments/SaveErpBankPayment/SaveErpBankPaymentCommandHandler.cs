using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.BankPayments.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.BankPayments.SaveErpBankPayment
{
    public class SaveErpBankPaymentCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpBankPaymentDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IBankPaymentMapRepository _bankPaymentMapRepository;
        private readonly ErpBankPaymentSaver _erpBankPaymentSaver;

        public SaveErpBankPaymentCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IBankPaymentMapRepository bankPaymentMapRepository, 
            ErpBankPaymentSaver erpBankPaymentSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _bankPaymentMapRepository = bankPaymentMapRepository;
            _erpBankPaymentSaver = erpBankPaymentSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpBankPaymentDto> command, CancellationToken cancellationToken)
        {
            var payment = command.Value;

            MappingInfo mappingInfo = null;
            foreach (var clientOrder in payment.ClientOrders)
            {
                mappingInfo = await _erpBankPaymentSaver.GetMappingInfo(clientOrder.ClientId);
                if (!mappingInfo.IsMappingFull)
                {
                    await SaveNotFullMapping(payment, mappingInfo.Why);
                    break;
                }
            }

            if (mappingInfo != null && !mappingInfo.IsMappingFull)
            {
                return new Unit();
            }

            var paymentMappings = await _bankPaymentMapRepository.GetRangeByErpIdAsync(payment.Id);

            var paymentIdsWithoutClientOrder = paymentMappings.Where(e => !e.ClientOrderId.HasValue).Select(e => e.LegacyId);
            await _erpBankPaymentSaver.DeletePaymentOrderWithoutClientOrder(paymentIdsWithoutClientOrder);

            var currentClientOrderIds = payment.ClientOrders.Where(e => e.ClientOrderId.HasValue).Select(e=>(int)e.ClientOrderId);
            var clientOrderIdsWithPayment = paymentMappings.Where(e => e.ClientOrderId.HasValue).Select(e=>(int)e.ClientOrderId);

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
                _erpBankPaymentSaver.InitErpObject(payment, paymentMapping, payment.ClientOrders.First(e=>e.ClientOrderId == clientOrderId));
                await _erpBankPaymentSaver.Update();
            }
            
            foreach(var paymentClientOrder in paymentClientOrdersForCreate)
            {
                _erpBankPaymentSaver.InitErpObject(payment, null, paymentClientOrder);
                await _erpBankPaymentSaver.Create(command.MessageId);
            }

            await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.BankPayment);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpBankPaymentDto payment, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                payment.Id,
                MappingTypes.BankPayment,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(payment)
            ));
        }
    }
}
