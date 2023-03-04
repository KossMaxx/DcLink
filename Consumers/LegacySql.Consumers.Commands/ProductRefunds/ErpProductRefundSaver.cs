using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Clients;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MessageBus.ProductRefunds.Import;

namespace LegacySql.Consumers.Commands.ProductRefunds
{
    public class ErpProductRefundSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IProductRefundMapRepository _productRefundMapRepository;

        private ErpProductRefundDto _entity;
        private ExternalMap _entityMapping;
        private ExternalMap _clientMapping;
        private ExternalMap _clientOrderMapping;
        private Dictionary<Guid, int> _productMappings = new Dictionary<Guid, int>();

        public ErpProductRefundSaver(
            IDbConnection db, 
            IClientMapRepository clientMapRepository, 
            IClientOrderMapRepository clientOrderMapRepository, 
            IProductMapRepository productMapRepository, 
            IProductRefundMapRepository productRefundMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _productMapRepository = productMapRepository;
            _productRefundMapRepository = productRefundMapRepository;
        }

        public void InitErpObject(ErpProductRefundDto entity, ExternalMap entityMapping)
        {
            _entity = entity;
            this._entityMapping = entityMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(_entity.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_entity.ClientId} не найден\n");
            }

            _clientOrderMapping = await _clientOrderMapRepository.GetByErpAsync(_entity.ClientOrderId);
            if (_clientOrderMapping == null)
            {
                why.Append($"Маппинг заказа id:{_entity.ClientOrderId} не найден\n");
            }

            if (_entity.Items.Any())
            {
                foreach (var item in _entity.Items)
                {
                    var productMapping = await _productMapRepository.GetByErpAsync(item.NomenclatureId);
                    if (productMapping == null)
                    {
                        why.Append($"Маппинг продукта id: {item.NomenclatureId} не найден\n");
                    }
                    else
                    {
                        if (!_productMappings.ContainsKey(item.NomenclatureId))
                        {
                            _productMappings.Add(item.NomenclatureId, productMapping.LegacyId);
                        }
                    }
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task SaveErpObject(Guid messageId)
        {
            if (_entityMapping == null)
            {
                await Create(messageId);
            }
            else
            {
                await Update();
            }
        }

        private async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertRefundSqlQuery = @"insert into [dbo].[ПН] 
                                            ([Дата],[klientId],[тип])
                                            values (@Date,@ClientId,@Type);
                                            select cast(SCOPE_IDENTITY() as int)";
                var newRefundId = (await _db.QueryAsync<int>(insertRefundSqlQuery, new
                {
                    Date = _entity.Date,
                    ClientId = _clientMapping.LegacyId,
                    Type = ProductRefundType.Return
                }, transaction)).FirstOrDefault();

                var insertRefundItemSqlQuery = @"insert into [dbo].[Приход]
                                                ([НомерПН],[КодТовара],[Количество])
                                                values (@RefundId,@ProductId,@Quantity)";
                foreach (var item in _entity.Items)
                {
                    await _db.ExecuteAsync(insertRefundItemSqlQuery, new
                    {
                        RefundId = newRefundId,
                        ProductId = _productMappings[item.NomenclatureId],
                        Quantity = item.Quantity
                    }, transaction);
                }

                var insertRelationSqlQuery = @"insert into [dbo].[connected_documents]
                                            ([type1],[doc1ID],[type2],[doc2ID])
                                            values (@TypeRefund,@RefundId,@TypeClientOrder,@ClientOrderId)";
                await _db.ExecuteAsync(insertRelationSqlQuery, new
                {
                    TypeRefund = (byte)ProductRefundType.Return,
                    RefundId = newRefundId,
                    TypeClientOrder = (byte)ProductRefundType.ClientOrder,
                    ClientOrderId = _clientOrderMapping.LegacyId
                }, transaction);

                transaction.Commit();

                await _productRefundMapRepository.SaveAsync(new ExternalMap(Guid.NewGuid(), newRefundId, _entity.Id));
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var updateRefundSqlQuery = @"update [dbo].[ПН] 
                                            set [Дата]=@Date,[klientId]=@ClientId
                                            where [НомерПН]=@RefundId";
                await _db.ExecuteAsync(updateRefundSqlQuery, new
                {
                    RefundId = _entityMapping.LegacyId,
                    Date = _entity.Date,
                    ClientId = _clientMapping.LegacyId,
                    
                }, transaction);

                var selectRefundProductsIdsSqlQuery = @"select [КодТовара] from [dbo].[Приход]
                                                    where [НомерПН]=@RefundId";
                var currentRefundProductsIds = await _db.QueryAsync<int>(selectRefundProductsIdsSqlQuery, new
                {
                    RefundId=_entityMapping.LegacyId
                }, transaction);

                var insertRefundItemSqlQuery = @"insert into [dbo].[Приход]
                                                ([НомерПН],[КодТовара],[Количество])
                                                values (@RefundId,@ProductId,@Quantity)";
                foreach (var item in _entity.Items.Where(e=>currentRefundProductsIds.All(cp=>cp != _productMappings[e.NomenclatureId])))
                {
                    await _db.ExecuteAsync(insertRefundItemSqlQuery, new
                    {
                        RefundId = _entityMapping.LegacyId,
                        ProductId = _productMappings[item.NomenclatureId],
                        Quantity = item.Quantity
                    }, transaction);
                }

                var updateRefundItemSqlQuery = @"update [dbo].[Приход]
                                                set [Количество]=@Quantity
                                                where [НомерПН]=@RefundId and [КодТовара]=@ProductId";
                foreach (var item in _entity.Items.Where(e => currentRefundProductsIds.Any(cp => cp == _productMappings[e.NomenclatureId])))
                {
                    await _db.ExecuteAsync(updateRefundItemSqlQuery, new
                    {
                        RefundId = _entityMapping.LegacyId,
                        ProductId = _productMappings[item.NomenclatureId],
                        Quantity = item.Quantity
                    }, transaction);
                }

                var currentRefundProductsIdsForDelete = currentRefundProductsIds.Where(e => !_productMappings.ContainsValue(e));
                foreach (var productId in currentRefundProductsIdsForDelete)
                {
                    await _db.ExecuteAsync(updateRefundItemSqlQuery, new
                    {
                        RefundId = _entityMapping.LegacyId,
                        ProductId = productId,
                        Quantity = 0
                    }, transaction);
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }
    }
}
