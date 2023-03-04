using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Penalties.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Penalties.SaveErpPenalty
{
    public class SaveErpPenaltyCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpPenaltyDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpPenaltySaver _penaltySaver;

        public SaveErpPenaltyCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpPenaltySaver penaltySaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _penaltySaver = penaltySaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpPenaltyDto> command, CancellationToken cancellationToken)
        {
            if (command.Value.Sum > 0)
            {
                command.Value.Sum = -command.Value.Sum;
            }

            var penalty = command.Value;
            _penaltySaver.InitErpObject(penalty);

            var mapInfo = await _penaltySaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(command, mapInfo.Why);
                return new Unit();
            }

            await _penaltySaver.Create();
            await _erpNotFullMappedRepository.RemoveAsync(penalty.Id, MappingTypes.Penalty);
            return new Unit();
        }

        private async Task SaveNotFullMapping(BaseSaveErpCommand<ErpPenaltyDto> command, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                command.Value.Id,
                MappingTypes.Penalty,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(command.Value)
            ));
        }
    }
}