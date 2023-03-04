using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroups;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ProductTypeCategoryGroups;
using MassTransit;
using MediatR;
using MessageBus.ProductTypeCategoryGroups.Export;
using MessageBus.ProductTypeCategoryGroups.Export.Add;
using MessageBus.ProductTypeCategoryGroups.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroupById
{
    public class PublishProductTypeCategoryGroupByIdCommandHandler : IRequestHandler<PublishProductTypeCategoryGroupByIdCommand>
    {
        private readonly ILegacyProductTypeCategoryGroupRepository _legacyProductCategoryGroupRepository;
        private readonly IProductTypeCategoryGroupMapRepository _productCategoryGroupMapRepository;
        private readonly IBus _bus;

        public PublishProductTypeCategoryGroupByIdCommandHandler(IProductTypeCategoryGroupMapRepository productCategoryGroupMapRepository,
            IBus bus, ILegacyProductTypeCategoryGroupRepository legacyProductCategoryGroupRepository,
            ILogger<PublishProductTypeCategoryGroupsCommand> logger,
            ICommandsHandlerManager handlerManager,
            ISagaLogger sagaLogger)
        {
            _productCategoryGroupMapRepository = productCategoryGroupMapRepository;
            _bus = bus;
            _legacyProductCategoryGroupRepository = legacyProductCategoryGroupRepository;
        }

        public async Task<Unit> Handle(PublishProductTypeCategoryGroupByIdCommand command, CancellationToken cancellationToken)
        {
            var group = await _legacyProductCategoryGroupRepository.Get(command.GroupId, cancellationToken);
            if (group == null)
            {
                throw new KeyNotFoundException("Тип продукта не найден");
            }

            if (group.IsChanged())
            {
                var groupDto = MapToDto(group);
                var message = new ChangeLegacyProductTypeCategoryGroupMessage
                {
                    MessageId = Guid.NewGuid(),
                    SagaId = Guid.NewGuid(),
                    Value = groupDto,
                    ErpId = group.Id.ExternalId.Value,
                };
                await _bus.Publish(message, cancellationToken);
            }

            if (group.IsNew())
            {
                var messageId = Guid.NewGuid();
                var groupDto = MapToDto(group);
                var message = new AddProductTypeCategoryGroupMessage
                {
                    MessageId = messageId,
                    SagaId = Guid.NewGuid(),
                    Value = groupDto
                };
                await _bus.Publish(message, cancellationToken);

                await _productCategoryGroupMapRepository.SaveAsync(new ProductTypeCategoryGroupMap(messageId, group.Id.InnerId, group.Name));
            }

            return new Unit();
        }

        private ProductTypeCategoryGroupDto MapToDto(ProductTypeCategoryGroup group)
        {
            return new ProductTypeCategoryGroupDto
            {
                LegacyId = group.Id.InnerId,
                Name = group.Name,
                NameUA = group.NameUA,
                Sort = group.Sort
            };
        }
    }
}
