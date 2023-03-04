using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Bonuses.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Bonuses.SaveErpBonus
{
    public class SaveErpBonusCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpBonusDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpBonusSaver _erpBonusSaver;

        public SaveErpBonusCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpBonusSaver erpBonusSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpBonusSaver = erpBonusSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpBonusDto> command, CancellationToken cancellationToken)
        {
            if (command.Value.Sum < 0)
            {
                command.Value.Sum = -command.Value.Sum;
            }

            var bonus = command.Value;
            _erpBonusSaver.InitErpObject(bonus);

            var mapInfo = await _erpBonusSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(command, mapInfo.Why);
                return new Unit();
            }

            await _erpBonusSaver.Create();
            await _erpNotFullMappedRepository.RemoveAsync(bonus.Id, MappingTypes.Bonus);
            return new Unit();
        }

        private async Task SaveNotFullMapping(BaseSaveErpCommand<ErpBonusDto> command, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                command.Value.Id,
                MappingTypes.Bonus,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(command.Value)
            ));
        }
    }
}