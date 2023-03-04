using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.ClientOrders
{
    public class ErpClientOrderSerialNumbersSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductMapRepository _productMapRepository;
        private ExternalMap _clientOrderMapping;
        private ErpClientOrderSerialNumbersDto _orderSerialNumbers;

        public ErpClientOrderSerialNumbersSaver(IDbConnection db,  
            IProductMapRepository productMapRepository)
        {
            _db = db;
            _productMapRepository = productMapRepository;
        }

        public void InitErpObject(ErpClientOrderSerialNumbersDto orderSerialNumbers, ExternalMap clientOrderMapping)
        {
            _orderSerialNumbers = orderSerialNumbers;
            _clientOrderMapping = clientOrderMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            if (_clientOrderMapping == null)
            {
                why.Append($"Маппинг заказа id: {_orderSerialNumbers.ClientOrderId} не найден\n");
            }

            if (_orderSerialNumbers.Products.Any())
            {
                foreach (var productSerialNumbers in _orderSerialNumbers.Products)
                {
                    var productMapping = await _productMapRepository.GetByErpAsync(productSerialNumbers.ProductId);
                    if (productMapping == null)
                    {
                        why.Append($"Маппинг продукта id: {productSerialNumbers.ProductId} не найден\n");
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

        public async Task Save()
        {
            foreach (var productSerialNumbers in _orderSerialNumbers.Products)
            {
                var productMapping = await _productMapRepository.GetByErpAsync(productSerialNumbers.ProductId);
                var getOperationNumberQuery = @"select [КодОперации] from [dbo].[Расход]
                                              where [НомерПН]=@OrderId and [КодТовара]=@ProductId";
                var operationNumber = (await _db.QueryAsync<int>(getOperationNumberQuery, new
                {
                    OrderId = _clientOrderMapping.LegacyId,
                    ProductId = productMapping.LegacyId
                })).FirstOrDefault();

                if (operationNumber == 0)
                {
                    throw new KeyNotFoundException($"Строка заказа id: {_clientOrderMapping.LegacyId} для товара с id: {productSerialNumbers.ProductId} не создана");
                }

                var getAllSerialNumbersByOperationQuery = @"select [Snom] from [dbo].[rushod]
                                                          where [Num]=@Operation";
                var allSerialNumbers = (await _db.QueryAsync<string>(getAllSerialNumbersByOperationQuery, new
                {
                    Operation = operationNumber
                })).ToList();

                _db.Open();
                using var transaction = _db.BeginTransaction();
                try
                {
                    //var numForDelete = allSerialNumbers.Except(productSerialNumbers.SerialNumbers);
                    //var deleteSerialNumbersByOperationQuery = @"delete from [dbo].[rushod]
                    //                                          where [Num]=@Operation and [Snom]=@SerialNumber";
                    //foreach (var serialNumber in numForDelete)
                    //{
                    //    await _db.ExecuteAsync(deleteSerialNumbersByOperationQuery, new
                    //    {
                    //        Operation = operationNumber,
                    //        SerialNumber = serialNumber
                    //    }, transaction);
                    //}

                    var numForInsert = productSerialNumbers.SerialNumbers.Except(allSerialNumbers);
                    var insertSerialNumber = @"insert into [dbo].[rushod]
                                             ([Num],[Snom]) 
                                             values (@Operation,@SerialNumber)";
                    foreach (var serialNumber in numForInsert)
                    {
                        await _db.ExecuteAsync(insertSerialNumber, new
                        {
                            Operation = operationNumber,
                            SerialNumber = serialNumber
                        }, transaction);
                    }

                    var setQuantityQuery = @"update [dbo].[Расход]
                                           set [Количество]=@Quantity
                                           where [НомерПН]=@OrderId and [КодТовара]=@ProductId";
                    await _db.ExecuteAsync(setQuantityQuery, new
                    {
                        OrderId = _clientOrderMapping.LegacyId,
                        ProductId = productMapping.LegacyId,
                        Quantity = productSerialNumbers.SerialNumbers.Count()
                    }, transaction);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _db.Close();
                }
            }
        }
    }
}
