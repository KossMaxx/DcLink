using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductMovings.Import;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.ProductMovings.SaveErpProductMoving
{
    public class SaveErpProductMovingCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpProductMovingDto>>
    {
        private readonly IProductMovingMapRepository _productMovingMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductMovingSaver _erpProductMovingSaver;

        public SaveErpProductMovingCommandHandler(
            IProductMovingMapRepository productMovingMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpProductMovingSaver erpProductMovingSaver)
        {
            _productMovingMapRepository = productMovingMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductMovingSaver = erpProductMovingSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpProductMovingDto> command, CancellationToken cancellationToken)
        {
            var productMoving = command.Value;
            var productMovingMapping = await _productMovingMapRepository.GetByErpAsync(productMoving.Id);
            _erpProductMovingSaver.InitErpObject(productMoving, productMovingMapping);

            var mappingInfo = await _erpProductMovingSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(command, mappingInfo.Why);
                return new Unit();
            }

            if (productMovingMapping == null)
            {
                await _erpProductMovingSaver.Create(command.MessageId);
            }
            else
            {
                await _erpProductMovingSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(productMoving.Id, MappingTypes.ProductMoving);

            return new Unit();
        }

        private async Task SaveNotFullMapping(BaseSaveErpCommand<ErpProductMovingDto> command, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                command.Value.Id,
                MappingTypes.ProductMoving,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(command.Value)
            ));
        }
    }
}
