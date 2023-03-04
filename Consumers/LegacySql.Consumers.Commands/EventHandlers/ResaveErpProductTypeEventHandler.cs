using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.ProductTypes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductTypeEventHandler : INotificationHandler<ResaveErpProductTypeEvent>
    {
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly ErpProductTypeSaver _erpProductTypeSaver;


        public ResaveErpProductTypeEventHandler(IProductTypeMapRepository productTypeMapRepository, 
            IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpProductTypeSaver erpProductTypeSaver)
        {
            _productTypeMapRepository = productTypeMapRepository;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpProductTypeSaver = erpProductTypeSaver;
        }

        public async Task Handle(ResaveErpProductTypeEvent notification, CancellationToken cancellationToken)
        {
            foreach (var type in notification.Messages)
            {
                var mappingInfo = await _erpProductTypeSaver.GetMappingInfo(type);
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                var productTypeMap = await _productTypeMapRepository.GetByErpAsync(type.Id);
                _erpProductTypeSaver.InitErpObject(type, productTypeMap);
                if (productTypeMap != null)
                {
                    await _erpProductTypeSaver.Update(Guid.NewGuid());
                }
                else
                {
                    await _erpProductTypeSaver.Create(Guid.NewGuid());
                }

                await _erpNotFullMappedRepository.RemoveAsync(type.Id, MappingTypes.ProductType);
            }
        }
    }
}
