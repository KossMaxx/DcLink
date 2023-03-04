using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using E2E.Tests.Infrastructure;
using Xunit;

namespace E2E.Tests
{
    public class SellingPriceTest : IClassFixture<ErpMbSqlDatabase>, IClassFixture<SqlDatabase>
    {
        private readonly SqlDatabase _sqlDb;
        private readonly ErpMbSqlDatabase _erpMbDb;

        public SellingPriceTest(SqlDatabase sqlDb, ErpMbSqlDatabase erpMbDb)
        {
            _sqlDb = sqlDb;
            _erpMbDb = erpMbDb;
        }

        [Theory]
        [InlineData(5, 20000)]
        //[InlineData(10, 2000)]
        //[InlineData(100, 1000)]
        //[InlineData(3000, 1200)]
        public async Task Latency_For_Several_Changed_SellingPrices(int count, int delay)
        {
            var selectDateTimeSqlQuery = $@"select ""MsgTimestamp"" from public.""NewSellingPrices"" order by ""MsgTimestamp"" desc limit 1";
            var dateOfTest = await _erpMbDb.Connection.QueryFirstOrDefaultAsync<DateTime?>(selectDateTimeSqlQuery);

            await ChangePricesAsync(count);

            Thread.Sleep(count * delay);


            var selectByDateSqlQuery = new StringBuilder(@"select count(*) from public.""NewSellingPrices""");
            if (dateOfTest.HasValue)
            {
                selectByDateSqlQuery.Append(@" where ""MsgTimestamp""::timestamp > @DateOfTest");
            }                           
            
            var newPricesCount = await _erpMbDb.Connection.QueryFirstAsync<int>(selectByDateSqlQuery.ToString(), new
            {
                DateOfTest = dateOfTest
            });

            var selectSqlQuery = @"select count(*) from public.""NewSellingPrices""
                                 where ""Confirmed"" = false";
            var notConfirmedCount = await _erpMbDb.Connection.QueryFirstAsync<int>(selectSqlQuery);

            Assert.True(newPricesCount > 0);
            Assert.Equal(0,notConfirmedCount);
        }

        private async Task ChangePricesAsync(int count)
        {
            var updateSqlQuery = @"update [dbo].[Товары] 
                                 set [DataLastPriceChange]=GETDATE()
                                 where [КодТовара] in (select top(@Count) [КодТовара] from [dbo].[lSQL_v_Товары]
                                 where [DataLastPriceChange]<cast(GETDATE() as date))";

            await _sqlDb.Connection.ExecuteAsync(updateSqlQuery, new
            {
                Count = count
            });
        }
    }
}
