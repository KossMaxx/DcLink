using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ClientOrder.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ClientOrders.SaveErpClientOrderSerialNumbers
{
    public class SaveErpClientOrderSerialNumbersCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClientOrderSerialNumbersDto>>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpClientOrderSerialNumbersSaver _clientOrderSerialNumbersSaver;

        public SaveErpClientOrderSerialNumbersCommandHandler( 
            IClientOrderMapRepository clientOrderMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpClientOrderSerialNumbersSaver clientOrderSerialNumbersSaver)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _clientOrderSerialNumbersSaver = clientOrderSerialNumbersSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClientOrderSerialNumbersDto> command, CancellationToken cancellationToken)
        {
            var serialNumbers = command.Value;
            var clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(serialNumbers.ClientOrderId);
            _clientOrderSerialNumbersSaver.InitErpObject(serialNumbers, clientOrderMapping);

            var mappingInfo = await _clientOrderSerialNumbersSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(serialNumbers, mappingInfo.Why);
                return new Unit();
            }

            await _clientOrderSerialNumbersSaver.Save();
            await _erpNotFullMappedRepository.RemoveAsync(serialNumbers.ClientOrderId, MappingTypes.ClientOrderSerialNumbers);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpClientOrderSerialNumbersDto serialNumbers, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                serialNumbers.ClientOrderId,
                MappingTypes.ClientOrderSerialNumbers,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(serialNumbers)
            ));
        }
    }
}
