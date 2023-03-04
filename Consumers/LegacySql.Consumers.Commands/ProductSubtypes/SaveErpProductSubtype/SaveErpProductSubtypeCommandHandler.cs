using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductSubtypes.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ProductSubtypes.SaveErpProductSubtype
{
    public class SaveErpProductSubtypeCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpProductSubtypeDto>>
    {
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductSubtypeSaver _erpProductSubtypeSaver;

        public SaveErpProductSubtypeCommandHandler(
            IProductSubtypeMapRepository productSubtypeMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpProductSubtypeSaver erpProductSubtypeSaver)
        {
            _productSubtypeMapRepository = productSubtypeMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductSubtypeSaver = erpProductSubtypeSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpProductSubtypeDto> command, CancellationToken cancellationToken)
        {
            var subtype = command.Value;
            var map = await _productSubtypeMapRepository.GetByErpAsync(subtype.Id);

            _erpProductSubtypeSaver.InitErpObject(subtype, map);

            var mappingInfo = await _erpProductSubtypeSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(subtype, mappingInfo.Why);
                return new Unit();
            }
            
            
            if (map == null)
            {
                await _erpProductSubtypeSaver.Create(command.MessageId);
            }
            else
            {
                await _erpProductSubtypeSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(subtype.Id, MappingTypes.ProductSubtype);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpProductSubtypeDto subtype, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                subtype.Id,
                MappingTypes.ProductSubtype,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(subtype)
            ));
        }
    }
}
