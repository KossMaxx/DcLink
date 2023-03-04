using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Clients.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Clients
{
    public class SyncMasterBalanceCommandHandler : IRequestHandler<SyncMasterBalanceCommand>
    {
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private PartnerSaver _erpPartnerSaver;

        public SyncMasterBalanceCommandHandler(IClientMapRepository clientMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            PartnerSaver erpPartnerSaver)
        {
            _clientMapRepository = clientMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpPartnerSaver = erpPartnerSaver;
        }

        public async Task<Unit> Handle(SyncMasterBalanceCommand command, CancellationToken cancellationToken)
        {
            var partner = command.Value;
            var clientMapping = await _clientMapRepository.GetByErpAsync(partner.Id);
            _erpPartnerSaver.InitErpObject(partner, clientMapping);

            var mappingInfo = await _erpPartnerSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(partner, mappingInfo.Why);
                return new Unit();
            }

            await _erpPartnerSaver.Save(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(partner.Id, MappingTypes.Partner);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpPartnerDto partner, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                partner.Id,
                MappingTypes.Partner,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(partner)
            ));
        }
    }
}
