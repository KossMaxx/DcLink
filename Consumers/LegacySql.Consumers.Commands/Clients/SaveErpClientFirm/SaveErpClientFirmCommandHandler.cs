using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Firms;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Clients.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Clients.SaveErpClientFirm
{
    public class SaveErpClientFirmCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpFirmDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IFirmMapRepository _firmMapRepository;
        private ErpClientFirmSaver _erpClientFirmSaver;

        public SaveErpClientFirmCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpClientFirmSaver erpClientFirmSaver, IFirmMapRepository firmMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpClientFirmSaver = erpClientFirmSaver;
            _firmMapRepository = firmMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpFirmDto> command, CancellationToken cancellationToken)
        {
            var firm = command.Value;
            var firmMapping = await _firmMapRepository.GetByErpAsync(firm.Id);
            _erpClientFirmSaver.InitErpObject(firm, firmMapping);

            var mappingInfo = await _erpClientFirmSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(firm, mappingInfo.Why);
                return new Unit();
            }

            await _erpClientFirmSaver.Save(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(firm.Id, MappingTypes.ClientFirm);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpFirmDto firm, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                firm.Id,
                MappingTypes.ClientFirm,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(firm)
            ));
        }
    }
}
