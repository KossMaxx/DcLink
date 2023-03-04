using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using MediatR;
using MessageBus;
using MessageBus.ProductPriceConditions.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.ProductPriceConditions.SaveErpProductPriceCondition
{
    public class SaveErpProductPriceConditionCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpProductPriceConditionDto>>
    {
        private readonly IDbConnection _db;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IProductPriceConditionMapRepository _productPriceConditionMapRepository;
        private ExternalMap _clientMapping;
        private ExternalMap _productMapping;
        private ExternalMap _productPriceConditionMapping;

        public SaveErpProductPriceConditionCommandHandler(IDbConnection db,
            IErpNotFullMappedRepository erpNotFullMappedRepository, IClientMapRepository clientMapRepository,
            IProductMapRepository productMapRepository, IProductPriceConditionMapRepository productPriceConditionMapRepository)
        {
            _db = db;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _clientMapRepository = clientMapRepository;
            _productMapRepository = productMapRepository;
            _productPriceConditionMapRepository = productPriceConditionMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpProductPriceConditionDto> command, CancellationToken cancellationToken)
        {
            var productPriceCondition = command.Value;
            _productPriceConditionMapping = await _productPriceConditionMapRepository.GetByErpAsync(productPriceCondition.Id);

            if (command.Value.Status == ImportStatus.Update)
            {
                var isMappingFull = await CheckMapping(command);
                if (!isMappingFull)
                {
                    return new Unit();
                }

                if (_productPriceConditionMapping == null)
                {
                    await Create(productPriceCondition, command.MessageId);
                }
                else
                {
                    await Update(productPriceCondition, _productPriceConditionMapping.LegacyId);
                }
            }
            else if (command.Value.Status == ImportStatus.Delete)
            {
                if (_productPriceConditionMapping == null)
                {
                    throw new ArgumentException($"Маппинг условий прайсов по типам c id:{productPriceCondition.Id} не найден\n");
                }

                await Delete(_productPriceConditionMapping);
            }
            else
            {
                throw new ArgumentException($"Неизвестный статус: {command.Value.Status}");
            }

            return new Unit();
        }

        private async Task Create(ErpProductPriceConditionDto dto, Guid messageGuid)
        {
            var insertQuery = @"insert into [dbo].[HidenTovars] 
                                ([klientID],[tovarID],[price],[validdate],[price_value])
                                values (@ClientId,@ProductId,@Price,@DateTo,@Value);
                                select cast(SCOPE_IDENTITY() as int)";
            
            var newLegacyId = (await _db.QueryAsync<int>(insertQuery, new
            {
                ClientId = _clientMapping.LegacyId,
                ProductId = _productMapping.LegacyId,
                dto.Price,
                dto.DateTo,
                dto.Value,
            })).FirstOrDefault();

            await _productPriceConditionMapRepository.SaveAsync(new ExternalMap(messageGuid, newLegacyId, dto.Id));
        }

        private async Task Update(ErpProductPriceConditionDto dto, int legacyId)
        {
            var updateQuery = @"update [dbo].[HidenTovars] 
                                set   [klientID]=@ClientId,
                                      [tovarID]=@ProductId,
                                      [price]=@Price,
                                      [validdate]=@DateTo,
                                      [price_value]=@Value
                                where [id]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = legacyId,
                ClientId = _clientMapping.LegacyId,
                ProductId = _productMapping.LegacyId,
                dto.Price,
                dto.DateTo,
                dto.Value,
            });
        }

        private async Task Delete(ExternalMap map)
        {
            var updateQuery = @"delete from [dbo].[HidenTovars] 
                                where [id]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = map.LegacyId,
            });
            
            await _productPriceConditionMapRepository.DeleteByIdAsync(map.Id);
        }

        private async Task<bool> CheckMapping(BaseSaveErpCommand<ErpProductPriceConditionDto> command)
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(command.Value.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{command.Value.ClientId} не найден\n");
            }

            _productMapping = await _productMapRepository.GetByErpAsync(command.Value.ProductId);
            if (_productMapping == null)
            {
                why.Append($"Маппинг товара id:{command.Value.ProductId} не найден\n");
            }

            var whyString = why.ToString();
            var ok = string.IsNullOrEmpty(whyString);
            if (!ok)
            {
                await SaveNotFullMapping(command, whyString);
            }

            return ok;
        }

        private async Task SaveNotFullMapping(BaseSaveErpCommand<ErpProductPriceConditionDto> command, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                command.Value.Id,
                MappingTypes.ProductPriceCondition,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(command.Value)
            ));
        }
    }
}