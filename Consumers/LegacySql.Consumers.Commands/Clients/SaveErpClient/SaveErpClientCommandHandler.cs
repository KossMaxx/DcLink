using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Clients.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Clients.SaveErpClient
{
    public class SaveErpClientCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClientDto>>
    {
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientSaver _erpClientSaver;

        public SaveErpClientCommandHandler(
            IClientMapRepository clientMapRepository,  
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientSaver erpClientSaver)
        {
            _clientMapRepository = clientMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpClientSaver = erpClientSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClientDto> command, CancellationToken cancellationToken)
        {
            var client = command.Value;
            var clientMapping = await _clientMapRepository.GetByErpAsync(client.Id);
            _erpClientSaver.InitErpObject(client, clientMapping);

            var mappingInfo = await _erpClientSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(client, mappingInfo.Why);
                return new Unit();
            }

            await _erpClientSaver.Save(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(client.Id, MappingTypes.Client);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpClientDto client, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                client.Id,
                MappingTypes.Client,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(client)
            ));
        }
    }
}
