using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Cashboxes.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Cashboxes.SaveErpCashboxPayment
{
    public class SaveErpCashboxPaymentCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpCashboxPaymentDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ICashboxPaymentMapRepository _cashboxPaymentMapRepository;
        private readonly ErpCashboxPaymentSaver _erpCashboxPaymentSaver;

        public SaveErpCashboxPaymentCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ICashboxPaymentMapRepository cashboxPaymentMapRepository, 
            ErpCashboxPaymentSaver erpCashboxPaymentSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _cashboxPaymentMapRepository = cashboxPaymentMapRepository;
            _erpCashboxPaymentSaver = erpCashboxPaymentSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpCashboxPaymentDto> command, CancellationToken cancellationToken)
        {
            var payment = command.Value;
            var paymentMapping = await _cashboxPaymentMapRepository.GetByErpAsync(payment.Id);
            _erpCashboxPaymentSaver.InitErpObject(payment, paymentMapping);
            
            var mappingInfo = await _erpCashboxPaymentSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(payment, mappingInfo.Why);
                return new Unit();
            }
            
            if (paymentMapping == null)
            {
                await _erpCashboxPaymentSaver.Create(command.MessageId);
            }
            else
            {
                await _erpCashboxPaymentSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(payment.Id, MappingTypes.CashboxPayment);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpCashboxPaymentDto payment, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                payment.Id,
                MappingTypes.CashboxPayment,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(payment)
            ));
        }
    }
}
