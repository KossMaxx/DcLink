using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Rejects;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpRejectReplacementCostEventHandler : INotificationHandler<ResaveErpRejectReplacementCostEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpRejectReplacementCostSaver _erpRejectRejectReplacementCostSaver;

        public ResaveErpRejectReplacementCostEventHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, ErpRejectReplacementCostSaver erpRejectRejectReplacementCostSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpRejectRejectReplacementCostSaver = erpRejectRejectReplacementCostSaver;
        }

        public async Task Handle(ResaveErpRejectReplacementCostEvent notification, CancellationToken cancellationToken)
        {
            foreach (var rejectReplacementCost in notification.Messages)
            {
                _erpRejectRejectReplacementCostSaver.InitErpObject(rejectReplacementCost);

                var mapInfo = await _erpRejectRejectReplacementCostSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpRejectRejectReplacementCostSaver.SaveErpObject();
                await _erpNotFullMappedRepository.RemoveAsync(rejectReplacementCost.RejectId, MappingTypes.RejectReplacementCost);
            }
        }
    }
}
