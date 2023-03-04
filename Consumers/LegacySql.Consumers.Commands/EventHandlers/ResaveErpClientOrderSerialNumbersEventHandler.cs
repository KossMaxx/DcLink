using System.Data;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ClientOrders;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpClientOrderSerialNumbersEventHandler : INotificationHandler<ResaveErpClientOrderSerialNumbersEvent>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientOrderSerialNumbersSaver _clientOrderSerialNumbersSaver;

        public ResaveErpClientOrderSerialNumbersEventHandler(
            IClientOrderMapRepository clientOrderMapRepository,  
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientOrderSerialNumbersSaver clientOrderSerialNumbersSaver)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _clientOrderSerialNumbersSaver = clientOrderSerialNumbersSaver;
        }

        public async Task Handle(ResaveErpClientOrderSerialNumbersEvent notification, CancellationToken cancellationToken)
        {
            foreach (var serialNumbers in notification.Messages)
            {
                var clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(serialNumbers.ClientOrderId);
                _clientOrderSerialNumbersSaver.InitErpObject(serialNumbers, clientOrderMapping);

                var mappingInfo = await _clientOrderSerialNumbersSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                   continue;
                }

                await _clientOrderSerialNumbersSaver.Save();
                await _erpNotFullMappedRepository.RemoveAsync(serialNumbers.ClientOrderId,MappingTypes.ClientOrderSerialNumbers);
            }
        }
    }
}
