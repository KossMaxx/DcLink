using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Clients;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Firms;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpClientFirmEventHandler : INotificationHandler<ResaveErpClientFirmEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IFirmMapRepository _firmMapRepository;
        private ErpClientFirmSaver _erpClientFirmSaver;

        public ResaveErpClientFirmEventHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientFirmSaver erpClientFirmSaver, 
            IFirmMapRepository firmMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpClientFirmSaver = erpClientFirmSaver;
            _firmMapRepository = firmMapRepository;
        }

        public async Task Handle(ResaveErpClientFirmEvent notification, CancellationToken cancellationToken)
        {
            foreach (var firm in notification.Messages)
            {
                var firmMapping = await _firmMapRepository.GetByErpAsync(firm.Id);
                _erpClientFirmSaver.InitErpObject(firm, firmMapping);

                var mappingInfo = await _erpClientFirmSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpClientFirmSaver.Save(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(firm.Id, MappingTypes.ClientFirm);
            }
        }
    }
}

