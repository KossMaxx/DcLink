using LegacySql.Commands.Shared;
using LegacySql.Data.Models;
using LegacySql.Domain.PartnerProductGroups;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.PartnerProductGroups.Export;
using MessageBus.PartnerProductGroups.Export.Add;
using MessageBus.PartnerProductGroups.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.PartnerProductGroups.PublishProductGroups
{
    internal class PublishPartnerProductGroupsCommandHandler : ManagedCommandHandler<PublishPartnerProductGroupsCommand>
    {
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILegacyPartnerProductGroupsRepository _legacyPartnerProductGroupsRepository;
        private readonly IPartnerProductGroupsMapRepository _partnerProductGroupsMapRepository;

        public PublishPartnerProductGroupsCommandHandler(
            ILogger<PublishPartnerProductGroupsCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISagaLogger sagaLogger,
            ISqlMessageFactory messageFactory,
            ILegacyPartnerProductGroupsRepository legacyPartnerProductGroupsRepository, 
            IPartnerProductGroupsMapRepository partnerProductGroupsMapRepository) : base(logger, manager)
        {
            _bus = bus;
            _sagaLogger = sagaLogger;
            _messageFactory = messageFactory;
            _legacyPartnerProductGroupsRepository = legacyPartnerProductGroupsRepository;
            _partnerProductGroupsMapRepository = partnerProductGroupsMapRepository;
        }

        public override async Task HandleCommand(PublishPartnerProductGroupsCommand command, CancellationToken cancellationToken)
        {
            var groups = _legacyPartnerProductGroupsRepository.GetAllAsync(cancellationToken);
            await foreach (var group in groups)
            {
                if (group.IsNew())
                {
                    var dto = new PartnerProductGroupDto
                    {
                        Code = group.Id.InnerId,
                        Title = group.Title
                    };
                    var message = _messageFactory.CreateNewEntityMessage<AddPartnerProductGroupsMessage, PartnerProductGroupDto>(dto);
                    await _bus.Publish(message, cancellationToken);

                    var mapping = new ExternalMap(message.MessageId, group.Id.InnerId);
                    await _partnerProductGroupsMapRepository.SaveAsync(mapping);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Code);
                }

                if (group.IsChanged())
                {
                    var dto = new PartnerProductGroupDto
                    {
                        Code = group.Id.InnerId,
                        Title = group.Title
                    };
                    var message = _messageFactory.CreateChangedEntityMessage<ChangePartnerProductGroupMessage, PartnerProductGroupDto>(group.Id.ExternalId.Value, dto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Code);
                }
            }
        }
    }
}
