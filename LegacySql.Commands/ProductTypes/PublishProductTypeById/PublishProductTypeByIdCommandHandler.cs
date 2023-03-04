using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypeCategories;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MassTransit;
using MediatR;
using MessageBus.ProductTypeCategoryGroups.Export;
using MessageBus.ProductTypes.Export.Add;
using MessageBus.ProductTypes.Export.Change;

namespace LegacySql.Commands.ProductTypes.PublishProductTypeById
{
    public class PublishProductTypeByIdCommandHandler : IRequestHandler<PublishProductTypeByIdCommand>
    {
        private readonly ILegacyProductTypeRepository _legacyProductTypeRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IBus _bus;
        private readonly IProductTypeCategoryMapRepository _productTypeCategoryMapRepository;
        private readonly IProductTypeCategoryParameterMapRepository _productTypeCategoryParameterMapRepository;
        private Dictionary<Guid, int> _categoriesDict;
        private Dictionary<Guid, int> _categoryParametersDict;


        public PublishProductTypeByIdCommandHandler(IProductTypeMapRepository productTypeMapRepository,
            IBus bus, ILegacyProductTypeRepository legacyProductTypeRepository,
            IProductTypeCategoryMapRepository productTypeCategoryMapRepository,
            IProductTypeCategoryParameterMapRepository productTypeCategoryParameterMapRepository)
        {
            _productTypeMapRepository = productTypeMapRepository;
            _bus = bus;
            _legacyProductTypeRepository = legacyProductTypeRepository;
            _productTypeCategoryMapRepository = productTypeCategoryMapRepository;
            _productTypeCategoryParameterMapRepository = productTypeCategoryParameterMapRepository;
            _categoriesDict = new Dictionary<Guid, int>();
            _categoryParametersDict = new Dictionary<Guid, int>();
        }

        public int ProductTypeId { get; }
        public async Task<Unit> Handle(PublishProductTypeByIdCommand command, CancellationToken cancellationToken)
        {
            var productType = await _legacyProductTypeRepository.Get(command.ProductTypeId, cancellationToken);
            if (productType == null)
            {
                throw new KeyNotFoundException("Тип продукта не найден");
            }

            if (productType.IsChanged())
            {
                var productTypeDto = MapToDto(productType);
                var message = new ChangeLegacyProductTypeMessage
                {
                    MessageId = Guid.NewGuid(),
                    SagaId = Guid.NewGuid(),
                    Value = productTypeDto,
                    ErpId = productType.Code.ExternalId.Value,
                    Categories = await GetChangeMessagesOfCategories(productType.Categories)
                };
                await _bus.Publish(message, cancellationToken);
                foreach (var categoryMessage in message.Categories)
                {
                    if (!categoryMessage.ErpId.HasValue)
                    {
                        await _productTypeCategoryMapRepository.SaveAsync(new ExternalMap(categoryMessage.MessageId, _categoriesDict[categoryMessage.MessageId]));
                    }

                    foreach (var categoryParameterMessage in categoryMessage.Parameters)
                    {
                        if (!categoryParameterMessage.ErpId.HasValue)
                        {
                            await _productTypeCategoryParameterMapRepository.SaveAsync(
                                new ExternalMap(categoryParameterMessage.MessageId,
                                    _categoryParametersDict[categoryParameterMessage.MessageId]));
                        }
                    }
                }
            }

            if (productType.IsNew())
            {
                var messageId = Guid.NewGuid();
                var productTypeDto = MapToDto(productType);
                var message = new AddProductTypeMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = productTypeDto,
                    Categories = await GetAddMessagesOfCategories(productType.Categories)
                };
                await _bus.Publish(message, cancellationToken);

                await _productTypeMapRepository.SaveAsync(new ProductTypeMap(messageId, productType.Code.InnerId, productType.Name));
                foreach (var categoryMessage in message.Categories)
                {
                    await _productTypeCategoryMapRepository.SaveAsync(new ExternalMap(categoryMessage.MessageId, _categoriesDict[categoryMessage.MessageId]));
                    foreach (var categoryParameterMessage in categoryMessage.Parameters)
                    {
                        await _productTypeCategoryParameterMapRepository.SaveAsync(
                            new ExternalMap(categoryParameterMessage.MessageId,
                                _categoryParametersDict[categoryParameterMessage.MessageId]));
                    }
                }
            }

            return new Unit();
        }

        private MessageBus.ProductTypes.Export.ProductTypeDto MapToDto(ProductType type)
        {
            return new MessageBus.ProductTypes.Export.ProductTypeDto
            {
                Code = type.Code.InnerId,
                Name = type.Name,
                FullName = type.FullName,
                IsGroupe = type.IsGroupe,
                MainId = type.MainId
            };
        }

        private async Task<IEnumerable<AddProductTypeCategoryMessage>> GetAddMessagesOfCategories(IEnumerable<ProductTypeCategory> categories)
        {
            var productTypeCategoryMessages = new List<AddProductTypeCategoryMessage>();
            foreach (var category in categories)
            {
                var categoryMapping = await _productTypeCategoryMapRepository.GetByLegacyAsync(category.Id.InnerId);
                var messageId = categoryMapping?.MapId ?? Guid.NewGuid();
                var categoryGroup =
                    new ProductTypeCategoryGroupDto() { LegacyId = category.Group.Id.InnerId, Name = category.Group.Name, NameUA = category.Group.NameUA, Sort = category.Group.Sort};
                productTypeCategoryMessages.Add(new AddProductTypeCategoryMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = new MessageBus.ProductTypes.Export.ProductTypeCategoryDto
                    {
                        Code = category.Id.InnerId,
                        Name = category.Name,
                        NameUA = category.NameUA,
                        Web = category.Web,
                        Web2 = category.Web2,
                        PriceTag = category.PriceTag,
                        Group = categoryGroup
                    },
                    Parameters = await GetAddMessagesOfCategoryParameters(category.Parameters)
                });
                _categoriesDict.Add(messageId, category.Id.InnerId);
            }

            return productTypeCategoryMessages;
        }

        private async Task<IEnumerable<AddProductTypeCategoryParameterMessage>> GetAddMessagesOfCategoryParameters(IEnumerable<ProductTypeCategoryParameter> parameters)
        {
            var productTypeCategoryParametersMessages = new List<AddProductTypeCategoryParameterMessage>();
            foreach (var parameter in parameters.ToList())
            {
                var categoryParameterMapping = await _productTypeCategoryParameterMapRepository.GetByLegacyAsync(parameter.Id.InnerId);
                var messageId = categoryParameterMapping?.MapId ?? Guid.NewGuid();
                productTypeCategoryParametersMessages.Add(new AddProductTypeCategoryParameterMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = new MessageBus.ProductTypes.Export.ProductTypeCategoryParameterDto
                    {
                        Code = parameter.Id.InnerId,
                        Name = parameter.Name,
                        NameUA = parameter.NameUA
                    }
                });
                _categoryParametersDict.Add(messageId, parameter.Id.InnerId);
            }

            return productTypeCategoryParametersMessages;
        }

        private async Task<IEnumerable<ChangeLegacyProductTypeCategoryMessage>> GetChangeMessagesOfCategories(IEnumerable<ProductTypeCategory> categories)
        {
            var productTypeCategoryMessages = new List<ChangeLegacyProductTypeCategoryMessage>();
            foreach (var category in categories)
            {
                var categoryMapping = await _productTypeCategoryMapRepository.GetByLegacyAsync(category.Id.InnerId);
                var messageId = categoryMapping?.MapId ?? Guid.NewGuid();
                var categoryGroup =
                    new ProductTypeCategoryGroupDto() { LegacyId = category.Group.Id.InnerId, Name = category.Group.Name, NameUA = category.Group.NameUA };
                productTypeCategoryMessages.Add(new ChangeLegacyProductTypeCategoryMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = new MessageBus.ProductTypes.Export.ProductTypeCategoryDto
                    {
                        Code = category.Id.InnerId,
                        Name = category.Name,
                        NameUA = category.NameUA,
                        Web = category.Web,
                        Web2 = category.Web2,
                        PriceTag = category.PriceTag,
                        Group = categoryGroup
                    },
                    Parameters = await GetChangeMessagesOfCategoryParameters(category.Parameters),
                    ErpId = categoryMapping?.ExternalMapId
                });
                _categoriesDict.Add(messageId, category.Id.InnerId);
            }

            return productTypeCategoryMessages;
        }

        private async Task<IEnumerable<ChangeLegacyProductTypeCategoryParameterMessage>> GetChangeMessagesOfCategoryParameters(IEnumerable<ProductTypeCategoryParameter> parameters)
        {
            var productTypeCategoryParametersMessages = new List<ChangeLegacyProductTypeCategoryParameterMessage>();
            foreach (var parameter in parameters.ToList())
            {
                var categoryParameterMapping = await _productTypeCategoryParameterMapRepository.GetByLegacyAsync(parameter.Id.InnerId);
                var messageId = categoryParameterMapping?.MapId ?? Guid.NewGuid();
                productTypeCategoryParametersMessages.Add(new ChangeLegacyProductTypeCategoryParameterMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = new MessageBus.ProductTypes.Export.ProductTypeCategoryParameterDto
                    {
                        Code = parameter.Id.InnerId,
                        Name = parameter.Name,
                        NameUA = parameter.NameUA
                    },
                    ErpId = categoryParameterMapping?.ExternalMapId
                });
                _categoryParametersDict.Add(messageId, parameter.Id.InnerId);
            }

            return productTypeCategoryParametersMessages;
        }
    }
}
