using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductRefunds.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ProductRefunds.SaveErpProductRefund
{
    public class SaveErpProductRefundCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpProductRefundDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IProductRefundMapRepository _productRefundMapRepository;
        private ErpProductRefundSaver _erpProductRefundSaver;

        public SaveErpProductRefundCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IProductRefundMapRepository productRefundMapRepository, 
            ErpProductRefundSaver erpProductRefundSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _productRefundMapRepository = productRefundMapRepository;
            _erpProductRefundSaver = erpProductRefundSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpProductRefundDto> command, CancellationToken cancellationToken)
        {
            var entity = command.Value;
            var productMapping = await _productRefundMapRepository.GetByErpAsync(entity.Id);

            _erpProductRefundSaver.InitErpObject(entity, productMapping);

            var mapInfo = await _erpProductRefundSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(entity, mapInfo.Why);
                return new Unit();
            }

            await _erpProductRefundSaver.SaveErpObject(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(entity.Id, MappingTypes.ProductRefund);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpProductRefundDto entity, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                entity.Id,
                MappingTypes.ProductRefund,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(entity)
            ));
        }
    }
}
