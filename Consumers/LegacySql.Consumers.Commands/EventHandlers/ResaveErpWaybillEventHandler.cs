using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Waybills;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Waybills;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpWaybillEventHandler : INotificationHandler<ResaveErpWaybillEvent>
    {
        private readonly IWaybillMapRepository _waybillMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpWaybillSaver _erpWaybillSaver;

        public ResaveErpWaybillEventHandler(
            IWaybillMapRepository waybillMapRepository,
            IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpWaybillSaver erpWaybillSaver)
        {
            _waybillMapRepository = waybillMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpWaybillSaver = erpWaybillSaver;
        }
        public async Task Handle(ResaveErpWaybillEvent notification, CancellationToken cancellationToken)
        {
            foreach (var entity in notification.Messages)
            {
                var entityMapping = await _waybillMapRepository.GetByErpAsync(entity.Id);
                _erpWaybillSaver.InitErpObject(entity, entityMapping);

                var mappingInfo = await _erpWaybillSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (entityMapping == null)
                {
                    await _erpWaybillSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpWaybillSaver.Update();
                }
                await _erpNotFullMappedRepository.RemoveAsync(entity.Id, MappingTypes.Waybill);
            }
        }
    }
}
