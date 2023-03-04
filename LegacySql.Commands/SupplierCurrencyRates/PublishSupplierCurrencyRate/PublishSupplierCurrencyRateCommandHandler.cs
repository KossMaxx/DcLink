using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Extensions;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using MassTransit;
using MessageBus.SupplierCurrencyRates.Export;
using MessageBus.SupplierCurrencyRates.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.SupplierCurrencyRates.PublishSupplierCurrencyRate
{
    public class PublishSupplierCurrencyRateCommandHandler : ManagedCommandHandler<PublishSupplierCurrencyRateCommand>
    {
        private readonly ILegacySupplierCurrencyRateRepository _supplierCurrencyRateRepository;
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishSupplierCurrencyRateCommandHandler(
            ILogger<PublishSupplierCurrencyRateCommandHandler> logger,
            ICommandsHandlerManager manager,
            ILegacySupplierCurrencyRateRepository supplierCurrencyRateRepository,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            IErpChangedRepository erpChangedRepository, 
            ISqlMessageFactory messageFactory) : base(logger, manager)
        {
            _supplierCurrencyRateRepository = supplierCurrencyRateRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _erpChangedRepository = erpChangedRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishSupplierCurrencyRateCommand command, CancellationToken cancellationToken)
        {
            var erpChangedOrders = (await _erpChangedRepository.GetAll(typeof(SupplierCurrencyRate).Name))
                .ToDictionary(e => e.LegacyId, e => e.Date);

            IAsyncEnumerable<SupplierCurrencyRate> rates;
            var lastChangeDates = new List<DateTime>();

            if (command.Id.HasValue)
            {
                rates = _supplierCurrencyRateRepository.GetSupplierCurrencyRateAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(SupplierCurrencyRate));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate.Value);
                }
                rates = _supplierCurrencyRateRepository.GetChangedSupplierCurrencyRatesAsync(lastChangedDate, cancellationToken);
            }

            await foreach (var rate in rates)
            {
                if (rate.Date.HasValue)
                {
                   lastChangeDates.Add(rate.Date.Value);
                }

                if (erpChangedOrders.ContainsKey(rate.Id) &&
                    await IsCheckErpChanged(rate.Id, rate.Date, erpChangedOrders))
                {
                    continue;
                }

                var mappingInfo = rate.IsMappingsFull();

                if (mappingInfo.IsMappingFull)
                {
                    var rateDto = MapToDto(rate);
                    var message = _messageFactory.CreateNewEntityMessage<AddSupplierCurrencyRateMessage, SupplierCurrencyRateDto>(rateDto);
                    await _bus.Publish(message, cancellationToken);
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(SupplierCurrencyRate), lastChangeDates.Max());
            }
        }

        private SupplierCurrencyRateDto MapToDto(SupplierCurrencyRate rate)
        {
            return new SupplierCurrencyRateDto
            {
                ClientId = rate.ClientId?.ExternalId.Value,
                Date = rate.Date,
                RateBn = rate.RateBn,
                RateNal = rate.RateNal,
                RateDdr = rate.RateDdr,
                ChangedByBot = rate.ChangedByBot,
                BalanceCurrency = rate.BalanceCurrencyId,
                IsSupplier = rate.IsSupplier,
                IsCustomer = rate.IsCustomer
            };
        }

        private async Task<bool> IsCheckErpChanged(int rateId, DateTime? rateChangedAt, Dictionary<int, DateTime> erpChangedRates)
        {
            var type = GetType();
            var entityName = type.GetEntityName();

            if (erpChangedRates[rateId] == rateChangedAt)
            {
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {rateId} Ignored because changed by ERP");
                return true;
            }

            await _erpChangedRepository.Delete(rateId, typeof(SupplierCurrencyRate).Name);
            _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {rateId} Delete from ErpChanged");

            return false;
        }
    }
}
