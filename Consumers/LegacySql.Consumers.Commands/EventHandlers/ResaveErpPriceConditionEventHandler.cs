using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.PriceConditions;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpPriceConditionEventHandler : INotificationHandler<ResaveErpPriceConditionEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IPriceConditionMapRepository _priceConditionMapRepository;
        private readonly IManufacturerMapRepository _manufacturerMapRepository;
        private readonly ErpPriceConditionSaver _erpPriceConditionSaver;

        public ResaveErpPriceConditionEventHandler(IPriceConditionMapRepository priceConditionMapRepository,  
            IManufacturerMapRepository manufacturerMapRepository, ErpPriceConditionSaver erpPriceConditionSaver)
        {
            _priceConditionMapRepository = priceConditionMapRepository;
            _manufacturerMapRepository = manufacturerMapRepository;
            _erpPriceConditionSaver = erpPriceConditionSaver;
        }

        public async Task Handle(ResaveErpPriceConditionEvent notification, CancellationToken cancellationToken)
        {
            foreach (var priceCondition in notification.Messages)
            {
                var priceConditionMapping = await _priceConditionMapRepository.GetByErpAsync(priceCondition.Id);

                var manufacturerMap = priceCondition.Vendor.HasValue
                    ? await _manufacturerMapRepository.GetByErpAsync(priceCondition.Vendor.Value)
                    : null;

                _erpPriceConditionSaver.InitErpObject(priceCondition, priceConditionMapping, manufacturerMap);

                var mappingInfo = await _erpPriceConditionSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                if (priceConditionMapping == null)
                {
                    await _erpPriceConditionSaver.Create(Guid.NewGuid());
                }
                else
                {
                    await _erpPriceConditionSaver.Update();
                }

                await _erpNotFullMappedRepository.RemoveAsync(priceCondition.Id, MappingTypes.PriceCondition);
            }
        }
    }
}