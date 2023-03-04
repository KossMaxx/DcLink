using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using MediatR;

namespace LegacySql.Commands.Tests.GenerateTestClientOrders
{
    public class GenerateTestClientOrdersCommandHandler : IRequestHandler<GenerateTestClientOrdersCommand>
    {
        private readonly LegacyDbConnection _sqlDb;
        private readonly AppDbConnection _legacySqlDb;

        public GenerateTestClientOrdersCommandHandler(LegacyDbConnection sqlDb, 
            AppDbConnection legacySqlDb)
        {
            _sqlDb = sqlDb;
            _legacySqlDb = legacySqlDb;
        }

        public async Task<Unit> Handle(GenerateTestClientOrdersCommand command, CancellationToken cancellationToken)
        {
            var sessionDateTime = DateTime.Now;

            var newOrders = await GetNewOrders(command.Count);

            var newOrdersIds = new List<int>();
            Parallel.ForEach(newOrders, order =>
            {
                order.Description = $"#{command.SessionId}# Тестовая сессия от {sessionDateTime:dd-MM-yy HH:mm:ss.fff}";
                newOrdersIds.Add(CreateOrder(order));
            });

            return new Unit();
        }

        private int CreateOrder(Order order)
        {
            int newOrderId;
            var sqlConnection = _sqlDb.Connection;
            sqlConnection.Open();
            using var transaction = sqlConnection.BeginTransaction();
            try
            {
                var insertOrderSqlQuery = @"insert into [dbo].[РН] ([Дата]
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
                                      ,[WebUserID]
                                      ,[Описание])
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
                                       @WebUserId,
                                       @Description);
								   select cast(SCOPE_IDENTITY() as int)";
                newOrderId = (sqlConnection.Query<int>(insertOrderSqlQuery, order, transaction))
                    .FirstOrDefault();

                var insertOrderItemSqlQuery = @"insert into [dbo].[Расход] ([НомерПН]
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
                foreach (var item in order.Items)
                {
                    item.ClientOrderId = newOrderId;
                    sqlConnection.Execute(insertOrderItemSqlQuery, item, transaction);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return newOrderId;
        }

        private async Task<IEnumerable<Order>> GetNewOrders(int count)
        {
            var actualProductsIds = (await GetActualProductsIds()).ToList();
            var actualWarehousesIds = (await GetActualWarehousesIds()).ToList();
            var selectOrdersSqlQuery = $@"with Orders as
	                                     (select top(@Count)
		                                    Orders.[НомерПН]
	                                     from [dbo].[РН] as Orders
	                                     inner join Расход on Orders.НомерПН = Расход.НомерПН and Расход.Количество > 0
                                         inner join lSQL_v_Товары on Расход.КодТовара = lSQL_v_Товары.КодТовара
	                                     where Orders.Дата < '2020-12-01' 
                                         and Orders.[Отдел] in ({string.Join(",", actualWarehousesIds.Select(g => $"'{g}'"))})
	                                     group by Orders.НомерПН)
                                         select 
                                             [РН].[НомерПН] as Id
                                            ,[РН].[Сумма] as Amount
                                            ,[РН].[Кол] as Quantity
                                            ,[РН].[customer_order_ID] as MarketplaceNumber
                                            ,[РН].[курс] as Rate
                                            ,[РН].[Отдел] as WarehouseId
                                            ,[РН].[колонка] as Kolonka
                                            ,[РН].[REZERVDATA] as ReserveDate
                                            ,[РН].[lasttime] as LastTime
                                            ,[РН].[DataBal] as BalanceDate
                                            ,[РН].[klientID] as ClientId
                                            ,[РН].[WebUserID] as WebUserId
                                            ,Расход.[НомерПН] as ClientOrderId
                                            ,Расход.[КодТипа] as ProductTypeId
                                            ,Расход.[КодТовара] as ProductId
                                            ,Расход.[марка] as Brand
                                            ,Расход.[Цена] as Price
                                            ,Расход.[КолЗ] as Quantity
                                            ,Расход.[ЦенаГРН] as PriceUAH
                                            ,Расход.[SS] as PriceSS
                                            ,Расход.[КолСВ] as ColCB
                                            ,Расход.[warranty] as Warranty
                                            ,Расход.[pricemin] as PriceMin
                                            ,Расход.[crosskurs] as CrossRate
                                            ,Расход.[bonuslevelID] as LevelBonusId
                                            ,Расход.[price_rozn] as PriceRozn
                                        from Orders
                                        inner join [dbo].РН on [dbo].РН.НомерПН = Orders.НомерПН
                                        inner join [dbo].Расход on [dbo].Расход.НомерПН = Orders.НомерПН and [dbo].Расход.Количество > 0";

            Random rnd = new Random();
            var orders = new List<Order>();
            await _sqlDb.Connection.QueryAsync<Order, OrderItem, Order>(selectOrdersSqlQuery, (o, i) =>
            {
                var order = orders.FirstOrDefault(e => e.Id == o.Id);

                if (actualProductsIds.All(e => e != i.ProductId))
                {
                    int randomIndex = rnd.Next(1, actualProductsIds.Count());
                    i.ProductId = actualProductsIds.ElementAt(randomIndex);
                }

                if (order == null)
                {
                    o.Items.Add(i);
                    orders.Add(o);
                }
                else
                {
                    order.Items.Add(i);
                }

                return order;
            }, new
            {
                Count = count
            }, splitOn: "ClientOrderId");

            return orders;
        }

        private async Task<IEnumerable<int>> GetActualWarehousesIds()
        {
            var selectSqlQuery = $@"select ""LegacyId"" from public.""WarehouseMaps""";
            return await _legacySqlDb.Connection.QueryAsync<int>(selectSqlQuery);
        }

        private async Task<IEnumerable<int>> GetActualProductsIds()
        {
            var selectSqlQuery = $@"select ""LegacyId"" from public.""ProductMaps""";
            return await _legacySqlDb.Connection.QueryAsync<int>(selectSqlQuery);
        }

        private class Order
        {
            public int Id { get; set; }
            public DateTime Date { get; } = DateTime.Now;
            public double? Amount { get; set; }
            public int? Quantity { get; set; }
            public string MarketplaceNumber { get; set; }
            public string Manager { get; } = "tqm";
            public double? Rate { get; set; }
            public int? WarehouseId { get; set; }
            public byte? Kolonka { get; set; }
            public DateTime? ReserveDate { get; set; }
            public DateTime? LastTime { get; set; }
            public DateTime? BalanceDate { get; set; }
            public int ClientId { get; set; }
            public int? WebUserId { get; set; }
            public string Description { get; set; }
            public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        }

        private class OrderItem
        {
            public int? ProductId { get; set; }
            public int? ProductTypeId { get; set; }
            public int? Quantity { get; set; }
            public int ClientOrderId { get; set; }
            public decimal? Price { get; set; }
            public decimal? PriceUAH { get; set; }
            public decimal? PriceSS { get; set; }
            public short? Warranty { get; set; }
            public string Brand { get; set; }
            public int? ColCB { get; set; }
            public decimal? PriceMin { get; set; }
            public decimal CrossRate { get; set; }
            public int? LevelBonusId { get; set; }
            public decimal? PriceRozn { get; set; }
        }
    }
}
