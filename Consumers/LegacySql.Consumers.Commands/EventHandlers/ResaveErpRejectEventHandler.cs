using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Rejects;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpRejectEventHandler : INotificationHandler<ResaveErpRejectEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpRejectSaver _erpRejectSaver;
        private readonly IRejectMapRepository _rejectMapRepository;

        public ResaveErpRejectEventHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpRejectSaver erpRejectSaver, 
            IRejectMapRepository rejectMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpRejectSaver = erpRejectSaver;
            _rejectMapRepository = rejectMapRepository;
        }

        public async Task Handle(ResaveErpRejectEvent notification, CancellationToken cancellationToken)
        {
            foreach (var reject in notification.Messages)
            {
                var rejectMapping = await _rejectMapRepository.GetByErpAsync(reject.Id);
                _erpRejectSaver.InitErpObject(reject, rejectMapping);

                var mapInfo = await _erpRejectSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpRejectSaver.SaveErpObject(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(reject.Id, MappingTypes.Reject);
            }
        }
    }
}
