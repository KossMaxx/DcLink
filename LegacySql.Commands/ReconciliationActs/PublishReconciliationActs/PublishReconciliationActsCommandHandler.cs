using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ReconciliationActs;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ReconciliationActs.Export;
using MessageBus.ReconciliationActs.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.ReconciliationActs.PublishReconciliationActs
{
    public class PublishReconciliationActsCommandHandler : ManagedCommandHandler<PublishReconciliationActsCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyReconciliationActRepository _legacyReconciliationActRepository;
        private readonly IReconciliationActMapRepository _reconciliationActMapRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishReconciliationActsCommandHandler(
            ILegacyReconciliationActRepository legacyReconciliationActRepository,
            IBus bus, 
            ILastChangedDateRepository lastChangedDateRepository,
            IReconciliationActMapRepository reconciliationActMapRepository, 
            INotFullMappedRepository notFullMappedRepository, 
            ILogger<PublishReconciliationActsCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _legacyReconciliationActRepository = legacyReconciliationActRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _reconciliationActMapRepository = reconciliationActMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishReconciliationActsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        public async Task Publish(PublishReconciliationActsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.ReconciliationAct)).ToList();
            
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            IAsyncEnumerable<ReconciliationAct> reconciliationActs = null;
            var lastChangeDates = new List<DateTime?>();

            if (command.Id.HasValue)
            {
                reconciliationActs = GetReconciliationActAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ReconciliationAct));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate);
                }
                reconciliationActs = _legacyReconciliationActRepository.GetChangedReconciliationActsAsync(lastChangedDate, notFullMappingIds, cancellationToken);
            }

            await foreach (var reconciliationAct in reconciliationActs.WithCancellation(cancellationToken))
            {
                if (reconciliationAct.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(reconciliationAct.ChangedAt.Value);
                }

                var mappingInfo = reconciliationAct.IsMappingsFull();

                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(reconciliationAct.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(reconciliationAct.Id.InnerId, MappingTypes.ReconciliationAct, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (reconciliationAct.IsNew())
                    {
                        var reconciliationActDto = MapToDto(reconciliationAct);
                        var message = _messageFactory.CreateNewEntityMessage<AddReconciliationActMessage, ReconciliationActDto>(reconciliationActDto);
                        await _bus.Publish(message, cancellationToken);
                        await _reconciliationActMapRepository.SaveAsync(new ExternalMap(message.MessageId, reconciliationAct.Id.InnerId));
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(reconciliationAct.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(reconciliationAct.Id.InnerId, MappingTypes.ReconciliationAct));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(ReconciliationAct), lastChangeDates.Max().Value);
            }
        }

        private MessageBus.ReconciliationActs.Export.ReconciliationActDto MapToDto(ReconciliationAct reconciliationAct)
        {
            return new MessageBus.ReconciliationActs.Export.ReconciliationActDto
            {
                Sum = reconciliationAct.Sum,
                ClientId = reconciliationAct.ClientId?.ExternalId,
                IsApproved = reconciliationAct.IsApproved,
            };
        }

        private async IAsyncEnumerable<ReconciliationAct> GetReconciliationActAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var reconciliationAct = await _legacyReconciliationActRepository.GetReconciliationActAsync(id, cancellationToken);

            if (reconciliationAct == null)
            {
                throw new KeyNotFoundException("Сверочный акт не найден");
            }

            yield return reconciliationAct;
        }
    }
}