using LegacySql.Domain.MovementOrders;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.MovementOrders.Import;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.MovementOrders.SaveErpMovementOrder
{
    public class SaveErpMovementOrderHandler : IRequestHandler<BaseSaveErpCommand<ErpMovementOrderDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IMovementOrderMapRepository _movementOrderMapRepository;
        private ErpMovementOrderSaver _erpMovementOrderSaver;

        public SaveErpMovementOrderHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            IMovementOrderMapRepository movementOrderMapRepository, 
            ErpMovementOrderSaver erpMovementOrderSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _movementOrderMapRepository = movementOrderMapRepository;
            _erpMovementOrderSaver = erpMovementOrderSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpMovementOrderDto> command, CancellationToken cancellationToken)
        {
            var order = command.Value;
            var orderMapping = await _movementOrderMapRepository.GetByErpAsync(order.Id);
            _erpMovementOrderSaver.InitErpObject(order, orderMapping);

            var mappingInfo = await _erpMovementOrderSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(order, mappingInfo.Why);
                return new Unit();
            }

            await _erpMovementOrderSaver.Save(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(order.Id, MappingTypes.MovementOrder);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpMovementOrderDto order, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                order.Id,
                MappingTypes.MovementOrder,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(order)
            ));
        }
    }
}
