using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Rejects.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Rejects.SaveErpRejectReplacementCost
{
    public class SaveErpRejectReplacementCostCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpRejectReplacementCostDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpRejectReplacementCostSaver _erpRejectRejectReplacementCostSaver;

        public SaveErpRejectReplacementCostCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpRejectReplacementCostSaver erpRejectRejectReplacementCostSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpRejectRejectReplacementCostSaver = erpRejectRejectReplacementCostSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpRejectReplacementCostDto> command, CancellationToken cancellationToken)
        {
            var rejectReplacementCost = command.Value;

            _erpRejectRejectReplacementCostSaver.InitErpObject(rejectReplacementCost);

            var mapInfo = await _erpRejectRejectReplacementCostSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(rejectReplacementCost, mapInfo.Why);
                return new Unit();
            }

            await _erpRejectRejectReplacementCostSaver.SaveErpObject();
            await _erpNotFullMappedRepository.RemoveAsync(rejectReplacementCost.RejectId, MappingTypes.RejectReplacementCost);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpRejectReplacementCostDto cost, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                cost.RejectId,
                MappingTypes.RejectReplacementCost,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(cost)
            ));
        }
    }
}
