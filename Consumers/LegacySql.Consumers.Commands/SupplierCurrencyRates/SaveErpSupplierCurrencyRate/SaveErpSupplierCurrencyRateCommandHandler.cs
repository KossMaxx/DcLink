using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using MediatR;
using MessageBus.SupplierCurrencyRates.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.SupplierCurrencyRates.SaveErpSupplierCurrencyRate
{
    public class SaveErpSupplierCurrencyRateCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpSupplierCurrencyRateDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpSupplierCurrencyRateSaver _erpSupplierCurrencyRateSaver;

        public SaveErpSupplierCurrencyRateCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpSupplierCurrencyRateSaver erpSupplierCurrencyRateSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpSupplierCurrencyRateSaver = erpSupplierCurrencyRateSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpSupplierCurrencyRateDto> command, CancellationToken cancellationToken)
        {
            var rate = command.Value;

            _erpSupplierCurrencyRateSaver.InitErpObject(rate);

            var mapInfo = await _erpSupplierCurrencyRateSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(rate, mapInfo.Why);
                return new Unit();
            }

            await _erpSupplierCurrencyRateSaver.SaveErpObject(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(rate.ClientId, MappingTypes.SupplierCurrencyRate);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpSupplierCurrencyRateDto rate, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                rate.ClientId,
                MappingTypes.SupplierCurrencyRate,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(rate)
            ));
        }
    }
}
