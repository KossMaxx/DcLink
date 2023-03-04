using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Clients;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ProductPriceConditions.Import;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpProductPriceConditionEventHandler : INotificationHandler<ResaveErpProductPriceConditionEvent>
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IProductPriceConditionMapRepository _productPriceConditionMapRepository;
        private ExternalMap _clientMapping;
        private ExternalMap _productMapping;

        public ResaveErpProductPriceConditionEventHandler(IDbConnection db, 
            IProductMapRepository productMapRepository, IClientMapRepository clientMapRepository, IProductPriceConditionMapRepository productPriceConditionMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _productPriceConditionMapRepository = productPriceConditionMapRepository;
            _productMapRepository = productMapRepository;
        }

        public async Task Handle(ResaveErpProductPriceConditionEvent notification, CancellationToken cancellationToken)
        {
            foreach (var productPriceCondition in notification.Messages)
            {
                var isMappingFull = await CheckMapping(productPriceCondition);
                if (!isMappingFull)
                {
                    continue;
                }

                var _productPriceConditionMapping = await _productPriceConditionMapRepository.GetByErpAsync(productPriceCondition.Id);
                if (_productPriceConditionMapping == null)
                {
                    await Create(productPriceCondition);
                }
                else
                {
                    await Update(productPriceCondition, _productPriceConditionMapping.LegacyId);
                }
            }
        }

        private async Task Create(ErpProductPriceConditionDto dto)
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

            await _productPriceConditionMapRepository.SaveAsync(new ExternalMap(Guid.NewGuid(), newLegacyId, dto.Id));
        }
        
        private async Task Update(ErpProductPriceConditionDto dto, long legacyId)
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

        private async Task<bool> CheckMapping(ErpProductPriceConditionDto dto)
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(dto.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{dto.ClientId} не найден\n");
            }

            _productMapping = await _productMapRepository.GetByErpAsync(dto.ProductId);
            if (_productMapping == null)
            {
                why.Append($"Маппинг товара id:{dto.ProductId} не найден\n");
            }

            return string.IsNullOrEmpty(why.ToString());
        }
    }
}