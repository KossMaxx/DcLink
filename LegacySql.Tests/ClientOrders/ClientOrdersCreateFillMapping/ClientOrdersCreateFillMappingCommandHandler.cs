using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Legacy.Data;
using MediatR;
using Microsoft.Data.SqlClient;

namespace LegacySql.Tests.ClientOrders.ClientOrdersCreateFillMapping
{
    public class ClientOrdersCreateFillMappingCommandHandler : IRequestHandler<ClientOrdersCreateFillMappingCommand>
    {
        private readonly IDbConnection _legacyDb;
        private IEnumerable<IGrouping<int, ClientOrderItem>> _clientOrdersItems;
        public ClientOrdersCreateFillMappingCommandHandler(IDbConnection legacyDb)
        {
            _legacyDb = legacyDb;
        }

        public async Task<Unit> Handle(ClientOrdersCreateFillMappingCommand command, CancellationToken cancellationToken)
        {
            var clientOrdes = (await GetClientOrders(command.Count)).ToList();
            _clientOrdersItems = await GetClientOrdersItems(clientOrdes.Select(e => e.Id));
            await CreateClientOrders(clientOrdes);
            return new Unit();
        }

        private async Task<IEnumerable<ClientOrder>> GetClientOrders(int count)
        {
            var selectSqlQuery = @"select top(@Count) 
                                       [НомерПН] as Id
                                      ,[Дата] as Date
                                      ,[Сумма] as Amount
                                      ,[Кол] as Quantity
	                                  ,[customer_order_ID] as MarketplaceNumber
	                                  ,[менеджер] as Manager
                                      ,[курс] as Rate
                                      ,[Отдел] as WarehouseId
                                      ,[колонка] as Kolonka
                                      ,[REZERVDATA] as ReserveDate
                                      ,[lasttime] as LastTime
                                      ,[DataBal] as BalanceDate
                                      ,[klientID] as ClientId
                                      ,[WebUserID] as WebUserId
                                  from [dbo].[РН]
                                  where (select count(*) from [dbo].[Расход] where [НомерПН]=[dbo].[РН].[НомерПН]) > 0
                                  order by [DataSozd] desc";
            var orders = await _legacyDb.QueryAsync<ClientOrder>(selectSqlQuery, new
            {
                Count = count
            });

            return orders;
        }

        private async Task<IEnumerable<IGrouping<int,ClientOrderItem>>> GetClientOrdersItems(IEnumerable<int> clientOrdersIds)
        {
            var selectSqlQuery = $@"select 
                                        [КодОперации] as Id
                                       ,[НомерПН] as ClientOrderId
                                       ,[КодТипа] as ProductTypeId
                                       ,[КодТовара] as ProductId
                                       ,[марка] as Brand
                                       ,[Цена] as Price
                                       ,[КолЗ] as Quantity
                                       ,[ЦенаГРН] as PriceUAH
                                       ,[SS] as PriceSS
                                       ,[КолСВ] as ColCB
                                       ,[warranty] as Warranty
                                       ,[pricemin] as PriceMin
                                       ,[crosskurs] as CrossRate
                                       ,[bonuslevelID] as LevelBonusId
                                       ,[price_rozn] as PriceRozn
                                   from [skl2008_back].[dbo].[Расход] 
                                   where НомерПН in ({string.Join(",", clientOrdersIds.Select(g => $"'{g}'"))})";
            var ordersItems = await _legacyDb.QueryAsync<ClientOrderItem>(selectSqlQuery);

            return ordersItems.GroupBy(e => e.ClientOrderId);
        }

        private async Task CreateClientOrders(IEnumerable<ClientOrder> clientOrdes)
        {
            var insertSqlQuery = @"insert into [dbo].[РН] ([Дата]
                                      ,[Сумма]
                                      ,[Кол]
	                                  ,[customer_order_ID]
	                                  ,[менеджер]
                                      ,[курс]
                                      ,[Отдел]
                                      ,[колонка]
                                      ,[REZERVDATA]
                                      ,[lasttime]
                                      ,[DataBal]
                                      ,[klientID]
                                      ,[WebUserID])
								   values (@Date,
                                       @Amount,
                                       @Quantity,
                                       @MarketplaceNumber,
                                       @Manager,
                                       @Rate,
                                       @WarehouseId,
                                       @Kolonka,
                                       @ReserveDate,
                                       @LastTime,
                                       @BalanceDate,
                                       @ClientId,
                                       @WebUserId);
								   select cast(SCOPE_IDENTITY() as int)";


            foreach (var clientOrder in clientOrdes)
            {
                _legacyDb.Open();
                using var transaction = _legacyDb.BeginTransaction();
                try
                {
                    var newOrderId = (await _legacyDb.QueryAsync<int>(insertSqlQuery, clientOrder, transaction)).FirstOrDefault();
                    await CreateClientOrderItems(newOrderId, _clientOrdersItems.FirstOrDefault(e => e.Key == clientOrder.Id), transaction);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
                finally
                {
                    _legacyDb.Close();
                }
            }
        }

        private async Task CreateClientOrderItems(int newOrderId, IGrouping<int, ClientOrderItem> clientOrderItems, IDbTransaction transaction)
        {
            var insertSqlQuery = @"insert into [dbo].[Расход] ([НомерПН]
                                       ,[КодТипа]
                                       ,[КодТовара] 
                                       ,[марка]
                                       ,[Цена]
                                       ,[КолЗ]
                                       ,[ЦенаГРН]
                                       ,[SS]
                                       ,[КолСВ]
                                       ,[warranty]
                                       ,[pricemin]
                                       ,[crosskurs]
                                       ,[bonuslevelID]
                                       ,[price_rozn])
                                   values (@ClientOrderId,
                                        @ProductTypeId,
                                        @ProductId,
                                        @Brand,
                                        @Price,
                                        @Quantity,
                                        @PriceUah,
                                        @PriceSS,
                                        @ColCB,
                                        @Warranty,
                                        @PriceMin,
                                        @CrossRate,
                                        @LevelBonusId,
                                        @PriceRozn)";
            foreach (var item in clientOrderItems)
            {
                item.ClientOrderId = newOrderId;
                await _legacyDb.ExecuteAsync(insertSqlQuery, item, transaction);
            }
        }
    }
}
