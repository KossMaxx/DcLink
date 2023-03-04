using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;
using LegacySql.Domain.SupplierPrice;
using MassTransit;
using MediatR;
using MessageBus.SupplierPrices.Export;
using MessageBus.SupplierPrices.Export.Add;

namespace LegacySql.Commands.SupplierPrices.GetInitialSipplierPrices
{
    public class GetInitialSupplierPricesCommandHandler : IRequestHandler<GetInitialSupplierPricesCommand>
    {
        private readonly ILegacyProductRepository _productRepository;
        private readonly ILegacySupplierPriceRepository _supplierPriceRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;

        public GetInitialSupplierPricesCommandHandler(ILegacyProductRepository productRepository, 
            ILegacySupplierPriceRepository supplierPriceRepository, 
            IBus bus, 
            ISqlMessageFactory messageFactory)
        {
            _productRepository = productRepository;
            _supplierPriceRepository = supplierPriceRepository;
            _bus = bus;
            _messageFactory = messageFactory;
        }

        public async Task<Unit> Handle(GetInitialSupplierPricesCommand command, CancellationToken cancellationToken)
        {
            var filteredProductsIds = command.ProductId.HasValue
                ? new List<int> { command.ProductId.Value }
                : (await _productRepository.GetProductsIdsAsync(cancellationToken)).ToList();

            var count = filteredProductsIds.Count();
            var portion = 10000;
            var cycleLimitation = (double)count / portion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                var prices = await _supplierPriceRepository.GetInitialSellingPricePackagesAsync(filteredProductsIds.Skip(i * portion).Take(portion), cancellationToken);
                foreach (var price in prices)
                {
                    if (price.Supplier.HasValue)
                    {
                        var pricePackageDto = MapToPackageDto(price);
                        if (pricePackageDto.SupplierPrices.Any())
                        {
                            var message =
                                _messageFactory
                                    .CreateNewEntityMessage<AddSupplierPriceMessage, SupplierPricePackageDto>(
                                        pricePackageDto);
                            await _bus.Publish(message, cancellationToken);
                        }
                    }
                }
            }

            return new Unit();
        }

        private SupplierPriceDto MapToDto(SupplierPrice price)
        {
            return new SupplierPriceDto
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

        private SupplierPricePackageDto MapToPackageDto(SupplierPricePackage pricePackage)
        {
            var packageDto = new SupplierPricePackageDto
            {
                Supplier = pricePackage.Supplier.Value
            };

            foreach (var price in pricePackage.SupplierPrices)
            {
                var isMappingFullInfo = price.IsMappingsFull();

                if (isMappingFullInfo.IsMappingFull)
                {
                    packageDto.SupplierPrices.Add(MapToDto(price));
                }
            }

            return packageDto;
        }
    }
}
