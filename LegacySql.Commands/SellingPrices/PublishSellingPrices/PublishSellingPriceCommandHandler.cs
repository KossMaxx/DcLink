using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Extensions;
using LegacySql.Domain.SellingPrices;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.SellingPrices.Export;
using MessageBus.SellingPrices.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.SellingPrices.PublishSellingPrices
{
    public class PublishSellingPriceCommandHandler : ManagedCommandHandler<PublishSellingPriceCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacySellingPriceRepository _legacySellingPriceRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishSellingPriceCommandHandler(
            IBus bus, 
            ILastChangedDateRepository lastChangedDateRepository, 
            ILegacySellingPriceRepository legacySellingPriceRepository, 
            ILogger<PublishSellingPriceCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            IErpChangedRepository erpChangedRepository, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _legacySellingPriceRepository = legacySellingPriceRepository;
            _erpChangedRepository = erpChangedRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishSellingPriceCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishSellingPriceCommand command, CancellationToken cancellationToken)
        {
            var erpChangedPrices = (await _erpChangedRepository.GetAll(typeof(SellingPrice).Name))
                .ToDictionary(e => e.LegacyId, e => e.Date);

            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(SellingPrice));

            List<DateTime> lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            var prices = _legacySellingPriceRepository.GetChangedSellingPricesAsync(lastChangedDate, cancellationToken);
            await foreach (var price in prices)
            {
                var priceData = price.CurrencyList.First();
                var priceDate = price.CurrencyList.Select(c => c.Date).Max();
                if (erpChangedPrices.ContainsKey(priceData.ProductId.InnerId) && await IsCheckErpChanged(priceData.ProductId.InnerId, priceDate, erpChangedPrices))
                {
                    continue;
                }

                foreach (var sellingPrice in price.CurrencyList)
                {
                    if (sellingPrice.Date.HasValue)
                    {
                        lastDate.Add(sellingPrice.Date.Value);
                    }
                }

                if (price.ProductId != null)
                {
                    var message = _messageFactory.CreateNewEntityMessage<AddSellingPriceMessage, SellingPricePackageDto>(MapToPackageDto(price));
                    await _bus.Publish(message, cancellationToken);
                }
            }

            if (lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(SellingPrice), lastDate.Max());
            }
        }

        private SellingPriceDto MapToDto(SellingPrice price)
        {
            return new SellingPriceDto
            {
                Date = price.Date,
                ProductId = price.ProductId.ExternalId.Value,
                Price = price.Price ?? decimal.Zero,
                ColumnId = price.ColumnId,
                Algorithm = price.Algorithm,
                Currency = price.Currency,
                PaymentType = price.PaymentType
            };
        }

        private SellingPricePackageDto MapToPackageDto(SellingPricePackage pricePackage)
        {
            var packageDto = new SellingPricePackageDto
            {
                ProductId = pricePackage.ProductId.Value
            };

            foreach (var price in pricePackage.CurrencyList)
            {
                var priceDto = new SellingPriceDto
                {
                    Date = price.Date,
                    ProductId = price.ProductId.ExternalId.Value,
                    Price = price.Price ?? decimal.Zero,
                    ColumnId = price.ColumnId,
                    Algorithm = price.Algorithm,
                    Currency = price.Currency,
                    PaymentType = price.PaymentType
                };
                packageDto.CurrencyList.Add(priceDto);
            }

            return packageDto;
        }

        private async Task<bool> IsCheckErpChanged(int productId, DateTime? priceChangedAt,
            Dictionary<int, DateTime> erpChangedPrices)
        {
            var type = GetType();
            var entityName = type.GetEntityName();

            if (erpChangedPrices[productId] == priceChangedAt)
            {
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {productId} Ignored because changed by ERP");
                return true;
            }

            await _erpChangedRepository.Delete(productId, typeof(SellingPrice).Name);
            _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {productId} Delete from ErpChanged");

            return false;
        }
    }
}
