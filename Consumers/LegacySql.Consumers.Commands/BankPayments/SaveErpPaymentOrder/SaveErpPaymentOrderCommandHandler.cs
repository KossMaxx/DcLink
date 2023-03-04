using LegacySql.Domain.BankPayments;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.BankPayments.Import;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.BankPayments.SaveErpPaymentOrder
{
    public class SaveErpPaymentOrderCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpPaymentOrderDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IPaymentOrderMapRepository _paymentOrderMapRepository;
        private readonly ErpPaymentOrderSaver _erpPaymentOrderSaver;

        public SaveErpPaymentOrderCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IPaymentOrderMapRepository paymentOrderMapRepository, 
            ErpPaymentOrderSaver erpPaymentOrderSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _paymentOrderMapRepository = paymentOrderMapRepository;
            _erpPaymentOrderSaver = erpPaymentOrderSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpPaymentOrderDto> command, CancellationToken cancellationToken)
        {
            var payment = command.Value;
            var paymentMapping = await _paymentOrderMapRepository.GetByErpAsync(payment.Id);
            _erpPaymentOrderSaver.InitErpObject(payment, paymentMapping);

            var mappingInfo = await _erpPaymentOrderSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(payment, mappingInfo.Why);
                return new Unit();
            }

            if (paymentMapping == null)
            {
                await _erpPaymentOrderSaver.Create(command.MessageId);
            }
            else
            {
                await _erpPaymentOrderSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.PaymentOrder);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpPaymentOrderDto payment, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                payment.Id,
                MappingTypes.PaymentOrder,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(payment)
            ));
        }
    }
}
