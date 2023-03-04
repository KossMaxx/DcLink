using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.ProductSubtypes;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductSubtypes.Import;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductSubtypeEventHandler : INotificationHandler<ResaveErpProductSubtypeEvent>
    {
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductSubtypeSaver _erpProductSubtypeSaver;

        public ResaveErpProductSubtypeEventHandler(
            IProductSubtypeMapRepository productSubtypeMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpProductSubtypeSaver erpProductSubtypeSaver)
        {
            _productSubtypeMapRepository = productSubtypeMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductSubtypeSaver = erpProductSubtypeSaver;
        }

        public async Task Handle(ResaveErpProductSubtypeEvent notification, CancellationToken cancellationToken)
        {
            foreach (var subtype in notification.Messages)
            {
                var map = await _productSubtypeMapRepository.GetByErpAsync(subtype.Id);

                _erpProductSubtypeSaver.InitErpObject(subtype, map);

                var mappingInfo = await _erpProductSubtypeSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (map == null)
                {
                    await _erpProductSubtypeSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpProductSubtypeSaver.Update();
                }

                await _erpNotFullMappedRepository.RemoveAsync(subtype.Id, MappingTypes.ProductSubtype);
            }
        }
    }
}
