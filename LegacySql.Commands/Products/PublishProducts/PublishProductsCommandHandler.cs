using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Products.Export;
using MessageBus.Products.Export.Add;
using MessageBus.Products.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;

namespace LegacySql.Commands.Products.PublishProducts
{
    public class PublishProductsCommandHandler : ManagedCommandHandler<PublishProductsCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyProductRepository _legacyProductRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishProductsCommandHandler(
            ILegacyProductRepository legacyProductRepository,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            IProductMapRepository productMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILogger<PublishProductsCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _legacyProductRepository = legacyProductRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _productMapRepository = productMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishProductsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        public async Task Publish(PublishProductsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.Product);

            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            IAsyncEnumerable<Product> products;
            var lastChangeDates = new List<DateTime?>();
            if (command.Id.HasValue)
            {
                products = GetProductAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Product));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate);
                }

                products = _legacyProductRepository.GetChangedProductsAsync(lastChangedDate, notFullMappingIds,
                    cancellationToken);
            }

           
            await foreach (var product in products)
            {
                if(product == null)
                {
                    return;
                }

                if (product.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(product.ChangedAt.Value);
                }

                var mappingInfo = product.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(product.Code.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(product.Code.InnerId,
                        MappingTypes.Product, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var productDto = MapToDto(product);
                    if (product.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyProductMessage, ProductDto>(product.Code.ExternalId.Value, productDto);
                        await _bus.Publish(message, cancellationToken);
                        
                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                    }

                    if (product.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddProductMessage, ProductDto>(productDto);
                        await _bus.Publish(message, cancellationToken);
                        await _productMapRepository.SaveAsync(new ExternalMap(message.MessageId, product.Code.InnerId));
                        
                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(product.Code.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(product.Code.InnerId, MappingTypes.Product));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(Product), lastChangeDates.Max().Value);
            }
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Code = product.Code.InnerId,
                Brand = product.Brand,
                WorkName = product.WorkName,
                SubtypeId = product.SubtypeId?.ExternalId,
                ProductCategoryId = product.ProductCategoryId?.ExternalId,
                ManufactureId = product.ManufactureId?.ExternalId,
                VendorCode = product.VendorCode,
                NomenclatureBarcode = product.NomenclatureBarcode,
                NameForPrinting = product.NameForPrinting,
                Weight = product.Weight,
                Volume = product.Volume,
                PackageQuantity = product.PackageQuantity,
                Guarantee = product.Guarantee,
                GuaranteeIn = product.GuaranteeIn,
                Unit = product.Unit,
                Vat = product.Vat,
                IsImported = product.IsImported,
                NomenclatureCode = product.NomenclatureCode,
                IsProductIssued = product.IsProductIssued,
                ContentUser = product.ContentUser,
                InStock = product.InStock,
                InReserve = product.InReserve,
                Pending = product.Pending,
                VideoUrl = product.VideoUrl,
                IsDistribution = product.IsDistribution,
                ScanMonitoring = product.ScanMonitoring,
                ScanHotline = product.ScanHotline,
                Game = product.Game,
                ManualRrp = product.ManualRrp,
                NotInvolvedInPricing = product.NotInvolvedInPricing,
                Monitoring = product.Monitoring,
                Price = product.Price,
                Markdown = product.Markdown,
                ProductTypeId = product.ProductTypeId?.ExternalId,
                ManufactureSiteLink = product.ManufactureSiteLink,
                ProductCountryIsoCode = product.ProductCountry,
                BrandCountryIsoCode = product.BrandCountry,
                Parameters = product.Parameters.Select(p => new ProductCategoryParameterDto
                {
                    CategoryId = p.CategoryId.ExternalId.Value,
                    ParameterId = p.ParameterId?.ExternalId
                }),
                Pictures = product.Pictures.Select(p => p.Url),
                Video = product.Video.Select(v => v.Url),
                ManagerId = product.ManagerId?.ExternalId,
                DescriptionRu = product.DescriptionRu,
                DescriptionUa = product.DescriptionUa,
                CurrencyId = product.CurrencyId,
                Height = product.Height,
                Width = product.Width,
                Depth = product.Depth,
                NonCashProductId = product.NonCashProductId,
                IsProduction = product.IsProduction
            };
        }

        private async IAsyncEnumerable<Product> GetProductAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var product = await _legacyProductRepository.GetProductAsync(id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Продукт не найден");
            }

            yield return product;
        }
    }
}