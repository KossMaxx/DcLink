using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ClientOrder.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ClientOrders.SaveErpClientOrder
{
    public class SaveErpClientOrderCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClientOrderDto>>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpClientOrderSaver _erpClientOrderSaver;
        private IEnumerable<ExternalMap> _clientOrderMapping;

        public SaveErpClientOrderCommandHandler( 
            IClientOrderMapRepository clientOrderMapRepository,  
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientOrderSaver erpClientOrderSaver)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpClientOrderSaver = erpClientOrderSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClientOrderDto> command, CancellationToken cancellationToken)
        {
            var order = command.Value;

            _clientOrderMapping = await _clientOrderMapRepository.GetRangeByErpAsync(order.Id);

            _erpClientOrderSaver.InitErpObject(order, _clientOrderMapping);

            var mapInfo = await _erpClientOrderSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                await SaveNotFullMapping(command, mapInfo.Why);
                return new Unit();
            }

            await _erpClientOrderSaver.SaveErpObject();
            await _erpNotFullMappedRepository.RemoveAsync(order.Id, MappingTypes.ClientOrder);
            return new Unit();
        }

        private async Task SaveNotFullMapping(BaseSaveErpCommand<ErpClientOrderDto> command, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                command.Value.Id,
                MappingTypes.ClientOrder,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(command.Value)
            ));
        }
    }
}
