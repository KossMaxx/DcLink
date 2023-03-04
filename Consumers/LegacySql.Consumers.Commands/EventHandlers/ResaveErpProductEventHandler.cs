using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Products;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductEventHandler : INotificationHandler<ResaveErpProductEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IProductMapRepository _productMapRepository;
        private ErpProductSaver _erpProductSaver;

        public ResaveErpProductEventHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, IProductMapRepository productMapRepository, ErpProductSaver erpProductSaver)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _productMapRepository = productMapRepository;
            _erpProductSaver = erpProductSaver;
        }
        
        public async Task Handle(ResaveErpProductEvent notification, CancellationToken cancellationToken)
        {
            foreach (var product in notification.Messages)
            {
                var productMapping = await _productMapRepository.GetByErpAsync(product.Id);
                
                _erpProductSaver.InitErpObject(product, productMapping);
                
                var isMappingFull = await _erpProductSaver.CheckMapping();
                if (!isMappingFull)
                {
                    continue;
                }

                await _erpProductSaver.SaveErpObject(Guid.NewGuid());

                await _erpNotFullMappedRepository.RemoveAsync(product.Id, MappingTypes.Product);
            }
        }
    }
}