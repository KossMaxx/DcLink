using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus;
using MessageBus.PriceConditions.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.PriceConditions.SaveErpPriceCondition
{
    public class SaveErpPriceConditionCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpPriceConditionDto>>
    {
        private readonly IDbConnection _db;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IPriceConditionMapRepository _priceConditionMapRepository;
        private readonly IManufacturerMapRepository _manufacturerMapRepository;
        private readonly ErpPriceConditionSaver _erpPriceConditionSaver;

        public SaveErpPriceConditionCommandHandler(IDbConnection db,
            IErpNotFullMappedRepository erpNotFullMappedRepository,
            IPriceConditionMapRepository priceConditionMapRepository,
            IManufacturerMapRepository manufacturerMapRepository,
            ErpPriceConditionSaver erpPriceConditionSaver)
        {
            _db = db;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _priceConditionMapRepository = priceConditionMapRepository;
            _manufacturerMapRepository = manufacturerMapRepository;
            _erpPriceConditionSaver = erpPriceConditionSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpPriceConditionDto> command, CancellationToken cancellationToken)
        {
            var priceCondition = command.Value;
            var priceConditionMapping = await _priceConditionMapRepository.GetByErpAsync(priceCondition.Id);

            var manufacturerMap = priceCondition.Vendor.HasValue
                ? await _manufacturerMapRepository.GetByErpAsync(priceCondition.Vendor.Value)
                : null;

            _erpPriceConditionSaver.InitErpObject(priceCondition, priceConditionMapping, manufacturerMap);

            if (priceCondition.Status == ImportStatus.Update)
            {
                var mappingInfo = await _erpPriceConditionSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    await SaveNotFullMapping(priceCondition, mappingInfo.Why);
                    return new Unit();
                }

                if (priceConditionMapping == null)
                {
                    await _erpPriceConditionSaver.Create(command.MessageId);
                }
                else
                {
                    await _erpPriceConditionSaver.Update();
                }
                await _erpNotFullMappedRepository.RemoveAsync(priceCondition.Id, MappingTypes.PriceCondition);
            }
            else if (priceCondition.Status == ImportStatus.Delete)
            {
                if (priceConditionMapping == null)
                {
                    throw new ArgumentException($"Маппинг условий прайсов по товарам c id:{priceCondition.ClientId} не найден\n");
                }

                await Delete(priceConditionMapping);
            }
            else
            {
                throw new ArgumentException($"Неизвестный статус: {priceCondition.Status}");
            }

            return new Unit();
        }

        private async Task Delete(ExternalMap map)
        {
            var updateQuery = @"delete from [dbo].[kolonkaByKlient] 
                                where [id]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = map.LegacyId,
            });

            await _priceConditionMapRepository.DeleteByIdAsync(map.Id);
        }

        private async Task SaveNotFullMapping(ErpPriceConditionDto priceCondition, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                priceCondition.Id,
                MappingTypes.PriceCondition,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(priceCondition)
            ));
        }
    }
}