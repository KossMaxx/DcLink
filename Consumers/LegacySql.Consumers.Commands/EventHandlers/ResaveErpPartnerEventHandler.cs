using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Clients;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpPartnerEventHandler : INotificationHandler<ResaveErpPartnerEvent>
    {
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private PartnerSaver _erpPartnerSaver;

        public ResaveErpPartnerEventHandler(IClientMapRepository clientMapRepository,  
            IErpNotFullMappedRepository erpNotFullMappedRepository, PartnerSaver erpPartnerSaver)
        {
            _clientMapRepository = clientMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpPartnerSaver = erpPartnerSaver;
        }

        public async Task Handle(ResaveErpPartnerEvent notification, CancellationToken cancellationToken)
        {
            foreach (var partner in notification.Messages)
            {
                var clientMapping = await _clientMapRepository.GetByErpAsync(partner.Id);
                _erpPartnerSaver.InitErpObject(partner, clientMapping);

                var mappingInfo = await _erpPartnerSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpPartnerSaver.Save(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(partner.Id, MappingTypes.Partner);
            }
        }
    }
}
