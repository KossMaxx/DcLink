using Dapper;
using E2E.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace E2E.Tests
{
    public class ClientOrderTest : IClassFixture<LegacySqlDatabase>, IClassFixture<SqlDatabase>
    {

        private readonly LegacySqlDatabase _legacySqlDb;
        private readonly SqlDatabase _sqlDb;

        public ClientOrderTest(LegacySqlDatabase legacySqlDb, SqlDatabase sqlDb)
        {
            _legacySqlDb = legacySqlDb;
            _sqlDb = sqlDb;
        }

        [Fact]
        public async Task Latency_For_NewOrderMapping_LessThen5Seconds()
        {
            int newOrderId = await CreateOrderAsync(Guid.NewGuid(), DateTime.Now);

            Thread.Sleep(5000);

            var getMappingSql = @"select ""ErpGuid"" 
                                from ""ClientOrderMaps"" 
                                where ""LegacyId"" = @SqlId";

            var getMappingResult =
                (await _legacySqlDb.Connection.QueryAsync<Guid?>(getMappingSql, new {SqlId = newOrderId})).ToList();

            Assert.Single(getMappingResult);
            Assert.True(getMappingResult.First().HasValue);
        }

        private async Task<int> CreateOrderAsync(Guid sessionId, DateTime sessionDateTime)
        {
            int newOrderId;
            var selectSqlQuery = @"select top(1) 
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
                                  order by [DataSozd] desc";
            var order = (await _sqlDb.Connection.QueryAsync(selectSqlQuery))
                .Select(e => new
                {
                    Id = (int) e.Id,
                    Date = (DateTime?) e.Date,
                    Amount = (double?) e.Amount,
                    Quantity = (int?) e.Quantity,
                    MarketplaceNumber = (string) e.MarketplaceNumber,
                    Manager = (string) e.Manager,
                    Rate = (double?) e.Rate,
                    WarehouseId = (int?) e.WarehouseId,
                    Kolonka = (byte?) e.Kolonka,
                    ReserveDate = (DateTime?) e.ReserveDate,
                    LastTime = (DateTime?) e.LastTime,
                    BalanceDate = (DateTime?) e.BalanceDate,
                    ClientId = (int) e.ClientId,
                    WebUserId = (int?) e.WebUserId,
                    Description = $"Тестовая сессия от {sessionDateTime:dd-MM-yy HH:mm:ss.fff} id {sessionId}"
                }).FirstOrDefault();

            _sqlDb.Connection.Open();
            using var transaction = _sqlDb.Connection.BeginTransaction();
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
                newOrderId = (await _sqlDb.Connection.QueryAsync<int>(insertOrderSqlQuery, order, transaction))
                    .FirstOrDefault();

                var selectProductsSqlQuery = @"select 
                                           [КодТипа] as ProductTypeId
                                           ,[КодТовара] as ProductId
                                           ,[марка] as Brand
                                           ,[Цена4] as Price
                                           ,[priceUAH] as PriceUah
                                           ,[SS] as PriceSS
                                           ,[Цена5] as PriceMin
                                           ,[Цена1] as PriceRozn
                                           from [dbo].[lSQL_v_Товары]
                                           where [КодТовара] in (select top(1) [tovID]
                                           from [dbo].[SkladFree]
                                           where skladID = 67 and Qfree > 3)";
                var product = (await _sqlDb.Connection.QueryAsync(selectProductsSqlQuery, null, transaction))
                    .Select(e => new
                    {
                        ClientOrderId = newOrderId,
                        ProductTypeId = (int?) e.ProductTypeId,
                        ProductId = (int?) e.ProductId,
                        Brand = (string) e.Brand,
                        Price = (decimal?) e.Price,
                        Quantity = 1,
                        PriceUah = (decimal?) e.PriceUah,
                        PriceSS = (decimal?) e.PriceSS,
                        ColCB = 1,
                        Warranty = (short?) 12,
                        PriceMin = (decimal?) e.PriceMin,
                        CrossRate = (decimal) 1,
                        LevelBonusId = 2,
                        PriceRozn = (decimal?) e.PriceRozn
                    })
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
                await _sqlDb.Connection.ExecuteAsync(insertOrderItemSqlQuery, product, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _sqlDb.Connection.Close();
            }

            return newOrderId;
        }

        [Theory]
        //[InlineData(5, 6000)]
        //[InlineData(10, 2000)]
        [InlineData(300, 1200)]
        //[InlineData(3000, 1200)]
        public async Task Latency_For_Several_New_Orders(int count, int delay)
        {
            var sessionId = Guid.NewGuid();
            var sessionDateTime = DateTime.Now;
            
            var newOrders = await GetNewOrders(count, sessionId, sessionDateTime);

            var newOrdersIds = new List<int>();
            Parallel.ForEach(newOrders, order =>
            {
                var sqlDb = new SqlDatabase();
                order.Description = $"#{sessionId}# Тестовая сессия от {sessionDateTime:dd-MM-yy HH:mm:ss.fff}";
                newOrdersIds.Add(CreateOrder(order, sqlDb));
            });

            Thread.Sleep(count * delay);

            var selectNewOrdersMappingsSqlQuery = $@"select * from public.""ClientOrderMaps"" 
                                                  where ""LegacyId"" in ({string.Join(",", newOrdersIds.Select(g => $"'{g}'"))})";
            var newOrdersMappings = (await _legacySqlDb.Connection.QueryAsync(selectNewOrdersMappingsSqlQuery))
                .Select(e => new
                {
                    Id = (Guid)e.Id,
                    MapGuid = (Guid)e.MapGuid,
                    ErpGuid = (Guid?)e.ErpGuid,
                    LegacyId = (long)e.LegacyId
                }).ToList();
            var newOrderMappingsWithoutErpGuid = newOrdersMappings.Where(e => !e.ErpGuid.HasValue);

            Assert.Equal(count, newOrdersMappings.Count);
            Assert.Empty(newOrderMappingsWithoutErpGuid);
        }

        private async Task<IEnumerable<Order>> GetNewOrders(int count, Guid sessionId, DateTime sessionDateTime)
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

        private int CreateOrder(Order order, SqlDatabase sqlDb)
        {
            int newOrderId;
            sqlDb.Connection.Open();
            using var transaction = sqlDb.Connection.BeginTransaction();
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
                newOrderId = (sqlDb.Connection.Query<int>(insertOrderSqlQuery, order, transaction))
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
                    sqlDb.Connection.Execute(insertOrderItemSqlQuery, item, transaction);
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
                sqlDb.Connection.Close();
            }

            return newOrderId;
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
