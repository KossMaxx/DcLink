using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using E2E.Tests.Infrastructure;
using Xunit;

namespace E2E.Tests
{
    public class SupplierPriceTest
    {
        private readonly SqlDatabase _sqlDb;
        private readonly ErpMbSqlDatabase _erpMbDb;

        public SupplierPriceTest(SqlDatabase sqlDb, ErpMbSqlDatabase erpMbDb)
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
            var selectDateTimeSqlQuery = $@"select ""MsgTimestamp"" from public.""NewSupplierPrices"" order by ""MsgTimestamp"" desc limit 1";
            var dateOfTest = await _erpMbDb.Connection.QueryFirstOrDefaultAsync<DateTime?>(selectDateTimeSqlQuery);

            await ChangePricesAsync(count);

            Thread.Sleep(count * delay);

            var selectByDateSqlQuery = new StringBuilder(@"select count(*) from public.""NewSupplierPrices""");
            if (dateOfTest.HasValue)
            {
                selectByDateSqlQuery.Append(@" where ""MsgTimestamp""::timestamp > @DateOfTest");
            }

            var newPricesCount = await _erpMbDb.Connection.QueryFirstAsync<int>(selectByDateSqlQuery.ToString(), new
            {
                DateOfTest = dateOfTest
            });

            var selectSqlQuery = @"select count(*) from public.""NewSupplierPrices""
                                 where ""Confirmed"" = false";
            var notConfirmedCount = await _erpMbDb.Connection.QueryFirstAsync<int>(selectSqlQuery);

            Assert.True(newPricesCount > 0);
            Assert.Equal(0, notConfirmedCount);
        }

        private async Task ChangePricesAsync(int count)
        {
            var updateSqlQuery = @"update [dbo].[Цены]
                                 set [KodTovara]=src.[KodTovara],[partner]=src.[partner],[price]=src.[partner],[nal]=src.[nal],[dataP]=getdate() 
                                 from (select top(@Count)* from [dbo].[Цены] order by dataP desc) src
                                 where [dbo].[Цены].[tovID] in (select top(@Count) [tovID] from [dbo].[Цены] order by dataP desc)";

            await _sqlDb.Connection.ExecuteAsync(updateSqlQuery, new
            {
                Count = count
            });
        }
    }
}
