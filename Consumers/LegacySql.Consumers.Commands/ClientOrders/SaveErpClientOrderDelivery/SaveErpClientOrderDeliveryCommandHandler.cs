using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ClientOrder.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ClientOrders.SaveErpClientOrderDelivery
{
    public class SaveErpClientOrderDeliveryCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClientOrderDeliveryDto>>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientOrderDeliverySaver _clientOrderDeliverySaver;

        public SaveErpClientOrderDeliveryCommandHandler(
            IClientOrderMapRepository clientOrderMapRepository, 
            ErpClientOrderDeliverySaver clientOrderDeliverySaver, 
            IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _clientOrderDeliverySaver = clientOrderDeliverySaver;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClientOrderDeliveryDto> command, CancellationToken cancellationToken)
        {
            var delivery = command.Value;
            var clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(delivery.OrderId);
            _clientOrderDeliverySaver.InitErpObject(delivery, clientOrderMapping);

            var mappingInfo = _clientOrderDeliverySaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(delivery, mappingInfo.Why);
                return new Unit();
            }

            await _clientOrderDeliverySaver.Save();
            await _erpNotFullMappedRepository.RemoveAsync(delivery.Id, MappingTypes.ClientOrderDelivery);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpClientOrderDeliveryDto delivery, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                delivery.OrderId,
                MappingTypes.ClientOrderDelivery,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(delivery)
            ));
        }
    }
}
