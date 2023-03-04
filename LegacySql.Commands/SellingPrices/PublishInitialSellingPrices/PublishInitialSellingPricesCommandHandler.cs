using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.SellingPrices;
using LegacySql.Domain.Shared;
using MassTransit;
using MediatR;
using MessageBus.SellingPrices.Export;
using MessageBus.SellingPrices.Export.Add;

namespace LegacySql.Commands.SellingPrices.PublishInitialSellingPrices
{
    public class PublishInitialSellingPricesCommandHandler : IRequestHandler<PublishInitialSellingPricesCommand>
    {
        private readonly ILegacySellingPriceRepository _sellingPriceRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;


        public PublishInitialSellingPricesCommandHandler(ILegacySellingPriceRepository sellingPriceRepository, IBus bus, ISqlMessageFactory messageFactory, ILastChangedDateRepository lastChangedDateRepository)
        {
            _sellingPriceRepository = sellingPriceRepository;
            _bus = bus;
            _messageFactory = messageFactory;
            _lastChangedDateRepository = lastChangedDateRepository;
        }

        public async Task<Unit> Handle(PublishInitialSellingPricesCommand command, CancellationToken cancellationToken)
        {
            var prices = _sellingPriceRepository.GetInitialSellingPricesAsync(command.ProductId, cancellationToken);
            var lastDate = new List<DateTime>();

            await foreach (var price in prices)
            {
                lastDate.AddRange(from sellingPrice in price.CurrencyList where sellingPrice.Date.HasValue select sellingPrice.Date.Value);

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

            return new Unit();
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
    }
}
