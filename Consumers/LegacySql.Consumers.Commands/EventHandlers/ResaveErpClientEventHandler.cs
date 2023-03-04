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
    public class ResaveErpClientEventHandler : INotificationHandler<ResaveErpClientEvent>
    {
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientSaver _erpClientSaver;

        public ResaveErpClientEventHandler(IClientMapRepository clientMapRepository,  
            IErpNotFullMappedRepository erpNotFullMappedRepository, ErpClientSaver erpClientSaver)
        {
            _clientMapRepository = clientMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpClientSaver = erpClientSaver;
        }

        public async Task Handle(ResaveErpClientEvent notification, CancellationToken cancellationToken)
        {
            foreach (var client in notification.Messages)
            {
                var clientMapping = await _clientMapRepository.GetByErpAsync(client.Id);
                _erpClientSaver.InitErpObject(client, clientMapping);

                var mappingInfo = await _erpClientSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpClientSaver.Save(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(client.Id, MappingTypes.Client);
            }
        }
    }
}
