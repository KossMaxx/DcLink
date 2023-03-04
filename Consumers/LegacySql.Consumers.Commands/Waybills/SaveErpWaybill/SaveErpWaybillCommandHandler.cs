using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Waybills;
using MediatR;
using MessageBus.Waybills.Import;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Waybills.SaveErpWaybill
{
    public class SaveErpWaybillCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpWaybillDto>>
    {
        private readonly IWaybillMapRepository _waybillMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpWaybillSaver _erpWaybillSaver;

        public SaveErpWaybillCommandHandler(
            IWaybillMapRepository waybillMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpWaybillSaver erpWaybillSaver)
        {
            _waybillMapRepository = waybillMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpWaybillSaver = erpWaybillSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpWaybillDto> command, CancellationToken cancellationToken)
        {
            var entity = command.Value;
            var entityMapping = await _waybillMapRepository.GetByErpAsync(entity.Id);
            _erpWaybillSaver.InitErpObject(entity, entityMapping);

            var mappingInfo = await _erpWaybillSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(entity, mappingInfo.Why);
                return new Unit();
            }

            if (entityMapping == null)
            {
                await _erpWaybillSaver.Create(command.MessageId);
            }
            else
            {
                await _erpWaybillSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(entity.Id, MappingTypes.Waybill);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpWaybillDto entity, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                entity.Id,
                MappingTypes.Waybill,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(entity)
            ));
        }
    }
}
