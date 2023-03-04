using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierPrice;
using MassTransit;
using MessageBus.SupplierPrices.Export;
using MessageBus.SupplierPrices.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.SupplierPrices.PublishSupplierPrices
{
    public class PublishSupplierPriceCommandHandler : ManagedCommandHandler<PublishSupplierPriceCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ILegacySupplierPriceRepository _legacySupplierPriceRepository;
        private readonly ILegacyProductRepository _productRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishSupplierPriceCommandHandler(
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILegacySupplierPriceRepository legacySupplierPriceRepository,
            ILegacyProductRepository productRepository,
            ICommandsHandlerManager handlerManager,
            ILogger<PublishSupplierPriceCommandHandler> logger,
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _legacySupplierPriceRepository = legacySupplierPriceRepository;
            _productRepository = productRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishSupplierPriceCommand command, CancellationToken cancellationToken)
        {
            await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishSupplierPriceCommand command, CancellationToken cancellationToken)
        {
            IEnumerable<SupplierPricePackage> prices = new List<SupplierPricePackage>();
            DateTime? lastDate = null;
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.SupplierPrice);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);

            var filteredProductsIds = command.ProductId.HasValue
                ? new List<int> { command.ProductId.Value }
                : (await _productRepository.GetProductsIdsAsync(cancellationToken)).ToList();

            if (command.ProductId.HasValue || command.Date.HasValue)
            {
                prices = await _legacySupplierPriceRepository.GetAllPackagesAsync(filteredProductsIds, command.Date, cancellationToken);
                if (!prices.Any())
                {
                    throw new KeyNotFoundException("Цен поставщиков не найдено");
                }

                await Publish(prices, notFullMappingsIdsDictionary, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(SupplierPrice));

                var count = filteredProductsIds.Count();
                var portion = 10000;
                var cycleLimitation = Math.Ceiling((double)count / portion);
                for (var i = 0; i < cycleLimitation; i++)
                {
                    (prices, lastDate) = await _legacySupplierPriceRepository.GetChangedSupplierPricePackagesAsync(filteredProductsIds.Skip(i * portion).Take(portion), lastChangedDate, notFullMappingIds, cancellationToken);
                    await Publish(prices, notFullMappingsIdsDictionary, cancellationToken);

                }
            }

            if (!command.ProductId.HasValue && !command.Date.HasValue && lastDate.HasValue)
            {
                await _lastChangedDateRepository.SetAsync(typeof(SupplierPrice), lastDate.Value);
            }
        }

        //private async Task Publish(IEnumerable<SupplierPrice> prices,
        //    Dictionary<int, int> notFullMappingsIdsDictionary, CancellationToken cancellationToken)
        //{
        //    foreach (var price in prices)
        //    {
        //        var isMappingFullInfo = price.IsMappingsFull();
        //        if (!isMappingFullInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(price.Id))
        //        {
        //            await _notFullMappedRepository.SaveAsync(new NotFullMapped(price.Id, MappingTypes.SupplierPrice, DateTime.Now, isMappingFullInfo.Why));
        //        }

        //        if (isMappingFullInfo.IsMappingFull)
        //        {
        //            var priceDto = MapToDto(price);
        //            var message = _messageFactory.CreateNewEntityMessage<AddSupplierPriceMessage, SupplierPriceDto>(priceDto);
        //            await _bus.Publish(message, cancellationToken);

        //            if (notFullMappingsIdsDictionary.ContainsKey(price.Id))
        //            {
        //                await _notFullMappedRepository.RemoveAsync(new NotFullMapped(price.Id, MappingTypes.SupplierPrice));
        //            }
        //        }
        //    }
        //}

        private async Task Publish(IEnumerable<SupplierPricePackage> prices,
            Dictionary<int, int> notFullMappingsIdsDictionary, CancellationToken cancellationToken)
        {
            foreach (var price in prices)
            {
                if (price.Supplier.HasValue)
                {
                    var pricePackageDto = await MapToPackageDto(price, notFullMappingsIdsDictionary, cancellationToken);
                    if (pricePackageDto.SupplierPrices.Any())
                    {
                        var message =
                            _messageFactory.CreateNewEntityMessage<AddSupplierPriceMessage, SupplierPricePackageDto>(
                                pricePackageDto);
                        await _bus.Publish(message, cancellationToken);
                    }
                }
            }
        }

        private MessageBus.SupplierPrices.Export.SupplierPriceDto MapToDto(SupplierPrice price)
        {
            return new MessageBus.SupplierPrices.Export.SupplierPriceDto
            {
                Date = price.Date,
                ProductId = price.ProductId.ExternalId.Value,
                SupplierId = price.SupplierId.ExternalId.Value,
                Price = price.Price ?? decimal.Zero,
                PriceRetail = price.PriceRetail ?? decimal.Zero,
                PriceDialer = price.PriceDialer ?? decimal.Zero,
                VendorCode = price.VendorCode,
                Monitor = price.Monitor,
                Currency = price.Currency,
                CurrencyRate = price.CurrencyRate,
                IsInStock = price.IsInStock,
                Url = price.Url,
                PaymentType = price.PaymentType,
                Nal = price.IsInStockText,
                PriceDate = price.PriceDate,
            };
        }

        private async Task<SupplierPricePackageDto> MapToPackageDto(SupplierPricePackage pricePackage, Dictionary<int, int> notFullMappingsIdsDictionary, CancellationToken cancellationToken)
        {
            var packageDto = new SupplierPricePackageDto
            {
                Supplier = pricePackage.Supplier.Value
            };

            foreach (var price in pricePackage.SupplierPrices)
            {
                var isMappingFullInfo = price.IsMappingsFull();
                switch (isMappingFullInfo.IsMappingFull)
                {
                    case false when !notFullMappingsIdsDictionary.ContainsKey(price.Id):
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(price.Id, MappingTypes.SupplierPrice, DateTime.Now, isMappingFullInfo.Why));
                        break;
                    case true:
                        {
                            packageDto.SupplierPrices.Add(MapToDto(price));

                            if (notFullMappingsIdsDictionary.ContainsKey(price.Id))
                            {
                                await _notFullMappedRepository.RemoveAsync(new NotFullMapped(price.Id, MappingTypes.SupplierPrice));
                            }

                            break;
                        }
                }
            }

            return packageDto;
        }
    }
}
