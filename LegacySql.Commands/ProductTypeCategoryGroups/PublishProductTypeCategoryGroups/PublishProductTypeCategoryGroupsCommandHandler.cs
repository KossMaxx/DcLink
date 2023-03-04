using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ProductTypeCategoryGroups;
using MassTransit;
using MessageBus.ProductTypeCategoryGroups.Export;
using MessageBus.ProductTypeCategoryGroups.Export.Add;
using MessageBus.ProductTypeCategoryGroups.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroups
{
    public class PublishProductTypeCategoryGroupsCommandHandler : ManagedCommandHandler<PublishProductTypeCategoryGroupsCommand>
    {
        private readonly ILegacyProductTypeCategoryGroupRepository _legacyProductCategoryGroupRepository;
        private readonly IProductTypeCategoryGroupMapRepository _productCategoryGroupMapRepository;
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;


        public PublishProductTypeCategoryGroupsCommandHandler(IProductTypeCategoryGroupMapRepository productCategoryGroupMapRepository,
            IBus bus, ILegacyProductTypeCategoryGroupRepository legacyProductCategoryGroupRepository,
            ILogger<PublishProductTypeCategoryGroupsCommand> logger,
            ICommandsHandlerManager handlerManager,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _productCategoryGroupMapRepository = productCategoryGroupMapRepository;
            _bus = bus;
            _legacyProductCategoryGroupRepository = legacyProductCategoryGroupRepository;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishProductTypeCategoryGroupsCommand command, CancellationToken cancellationToken)
        {
                await Publish(cancellationToken);
        }

        public async Task Publish(CancellationToken cancellationToken)
        {
            var groups = await _legacyProductCategoryGroupRepository.GetAllAsync(cancellationToken);

            foreach (var group in groups)
            {
                var groupDto = MapToDto(group);
                if (group.IsChanged())
                {
                    var message = new ChangeLegacyProductTypeCategoryGroupMessage
                    {
                        MessageId = Guid.NewGuid(),
                        SagaId = Guid.NewGuid(),
                        Value = groupDto,
                        ErpId = group.Id.ExternalId.Value,
                    };
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.LegacyId);
                }

                if (group.IsNew())
                {
                    var messageId = Guid.NewGuid();
                    var message = new AddProductTypeCategoryGroupMessage
                    {
                        MessageId = messageId,
                        SagaId = Guid.NewGuid(),
                        Value = groupDto,
                    };
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.LegacyId);

                    await _productCategoryGroupMapRepository.SaveAsync(new ProductTypeCategoryGroupMap(messageId, group.Id.InnerId, group.Name));
                }
            }
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