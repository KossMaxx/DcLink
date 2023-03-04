using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Rejects.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Rejects.SaveErpReject
{
    public class SaveErpRejectCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpRejectDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpRejectSaver _erpRejectSaver;
        private readonly IRejectMapRepository _rejectMapRepository;

        public SaveErpRejectCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpRejectSaver erpRejectSaver, 
            IRejectMapRepository rejectMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpRejectSaver = erpRejectSaver;
            _rejectMapRepository = rejectMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpRejectDto> command, CancellationToken cancellationToken)
        {
            var reject = command.Value;

            var rejectMapping = await _rejectMapRepository.GetByErpAsync(reject.Id);
            _erpRejectSaver.InitErpObject(reject, rejectMapping);

            var mapInfo = await _erpRejectSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(reject, mapInfo.Why);
                return new Unit();
            }

            await _erpRejectSaver.SaveErpObject(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(reject.Id, MappingTypes.Reject);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpRejectDto reject, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                reject.Id,
                MappingTypes.Reject,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(reject)
            ));
        }
    }
}
