using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Commands.ClientOrders.PublishClientOrder;
using LegacySql.Commands.Tests.GenerateTestClientOrders;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Legacy.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/tests")]
    public class TestController : ControllerBase
    {
        private readonly AppDbConnection _appDb;
        private readonly LegacyDbConnection _legacyDb;
        private readonly IMediator _mediator;

        public TestController(AppDbConnection appDb, LegacyDbConnection legacyDb, IMediator mediator)
        {
            _appDb = appDb;
            _legacyDb = legacyDb;
            _mediator = mediator;
        }

        [HttpGet("warehouse-stocks")]
        public async Task<ActionResult> GetWarehouseStocks(int page = 1, int pageSize = 50000)
        {
            if (pageSize > 50000)
            {
                pageSize = 50000;
            }

            var sqlQuery = $@"select SkladFree.*, Позиция as Position, Склады.sklad_desc 
                            from [dbo].[SkladFree] 
                            left join [dbo].[Товары] on Товары.КодТовара = tovID 
                            left join [dbo].[Склады] on Склады.sklad_ID = skladID 
                            order by [id] asc
                            offset {(page - 1) * pageSize} rows fetch next {pageSize} rows only";

            var result = (await _legacyDb.Connection.QueryAsync(sqlQuery)).Select(e => new
            {
                Id = e.id,
                ProductId = e.tovID,
                WarehouseId = e.skladID,
                Quantity = e.Qfree,
                e.Position,
                Description = e.sklad_desc
            });

            return Ok(result);
        }

        [HttpGet("compare-products")]
        public async Task<ActionResult> CompareProducts()
        {
            var sqlQuery = @"select [КодТовара] as Id, [Позиция] as Title, [КодТипа] as ProductTypeId 
                             from [dbo].[lSQL_v_Товары]";
            var legacyResult = (await _legacyDb.Connection.QueryAsync(sqlQuery, commandTimeout: 300)).Select(e => new
            {
                e.Id,
                e.Title,
                e.ProductTypeId
            });

            var mapQuery = $@"select ""LegacyId"" FROM public.""ProductMaps""";
            var mapResult = (await _appDb.Connection.QueryAsync(mapQuery)).Select(e => e.LegacyId).ToList();

            var notFullMapQuery = $@"select ""InnerId"" FROM public.""NotFullMapped"" 
                                     where ""Type""=1";
            var notFullMapResult = (await _appDb.Connection.QueryAsync(notFullMapQuery)).Select(e => e.InnerId).ToList();
            mapResult.AddRange(notFullMapResult);

            var result = legacyResult.Where(e => mapResult.All(id => id != e.Id));
            await WriteToFile("MissedProducts.json", JsonConvert.SerializeObject(new
            {
                count = result.Count(),
                value = result
            }));
            return Ok(result);
        }

        private async Task WriteToFile(string fileName, string data)
        {
            await using StreamWriter sw = new StreamWriter($"{Path.GetFullPath(fileName)}", false, System.Text.Encoding.Default);
            sw.WriteLine(data);
        }

        [HttpGet("products/remove-not-relevant-not-full-mappings")]
        public async Task<ActionResult> RemoveProductNotFullMappings()
        {
            var notFullMapQuery = $@"select ""InnerId"" FROM public.""NotFullMapped"" 
                                     where ""Type""=1";
            var notFullMapResult = (await _appDb.Connection.QueryAsync(notFullMapQuery)).Select(e => (long)e.InnerId).ToList();

            var sqlQuery = $@"select [КодТовара] as ProductId from lSQL_v_Товары";
            var sqlResult = (await _legacyDb.Connection.QueryAsync(sqlQuery)).Select(e => (long)e.ProductId).ToList(); ;

            var result = notFullMapResult.Except(sqlResult);

            foreach (var mapId in result)
            {
                await _appDb.Connection.ExecuteAsync(@"delete FROM public.""NotFullMapped"" 
                                                               where ""InnerId""=@mapId and ""Type""=1",
                    new
                    {
                        mapId
                    });
            }

            return Ok();
        }

        [HttpPost("client-orders/rpc/emulate-orders-load")]
        public async Task EmulateOrdersLoad(DateTime date, int hoursOffset = 0, int minutesOffset = 0)
        {
            var fromNow = DateTime.Now;
            var from = date.AddHours(fromNow.Hour + hoursOffset)
                .AddMinutes(fromNow.Minute + minutesOffset)
                .AddSeconds(fromNow.Second);

            while (from.Day == date.Day)
            {
                Thread.Sleep(2000);

                var toNow = DateTime.Now;
                var to = date.AddHours(toNow.Hour + hoursOffset)
                            .AddMinutes(toNow.Minute + minutesOffset)
                            .AddSeconds(toNow.Second);

                var sqlQuery = $@"select НомерПН 
                                  from РН 
                                  where DataSozd >= @From and DataSozd < @To";

                var orderIds = await _legacyDb.Connection.QueryAsync<int>(sqlQuery, new
                {
                    From = from.ToString("yyyy-MM-dd HH:mm:ss"),
                    To = to.ToString("yyyy-MM-dd HH:mm:ss")
                });

                foreach (var orderId in orderIds)
                {
                    try
                    {
                        var command = new PublishClientOrderCommand(orderId);
                        await _mediator.Send(command);
                    }
                    catch (Exception ex)
                    {
                        var currentException = ex;
                        var errorMsgBuilder = new StringBuilder(ex.Message);
                        if (currentException != null)
                        {
                            errorMsgBuilder.AppendLine(currentException.Message);
                        }
                        errorMsgBuilder.AppendLine(ex.StackTrace);
                        Log.Logger.Error($"Publish order {orderId} error: {errorMsgBuilder}");
                    }
                }

                from = to;
            }
        }

        [HttpPost("client-orders/rpc/load-orders-by-date")]
        public async Task LoadOrdersByDate(DateTime date)
        {
            var from = new DateTime(date.Year, date.Month, date.Day);
            var to = from.AddDays(1);

            var sqlQuery = $@"select НомерПН 
                                  from РН 
                                  where DataSozd >= @From and DataSozd < @To";

            var orderIds = await _legacyDb.Connection.QueryAsync<int>(sqlQuery, new
            {
                From = from.ToString("yyyy-MM-dd HH:mm:ss"),
                To = to.ToString("yyyy-MM-dd HH:mm:ss")
            });

            foreach (var orderId in orderIds)
            {
                try
                {
                    var command = new PublishClientOrderCommand(orderId);
                    await _mediator.Send(command);
                }
                catch (Exception ex)
                {
                    var currentException = ex;
                    var errorMsgBuilder = new StringBuilder(ex.Message);
                    if (currentException != null)
                    {
                        errorMsgBuilder.AppendLine(currentException.Message);
                    }
                    errorMsgBuilder.AppendLine(ex.StackTrace);
                    Log.Logger.Error($"Publish order {orderId} error: {errorMsgBuilder}");
                }
            }
        }

        [HttpGet("product-subtypes/is-mapping-exist")]
        public async Task<ActionResult> IsProductSubtypeMappingExist(Guid id)
        {
            var selectSubtypeMapQuery = $@"select * FROM public.""ProductSubtypeMaps"" 
                                        where ""ErpGuid""=@Id";
            var productSubtypeMap = await _appDb.Connection.QueryFirstOrDefaultAsync<ProductSubtypeMapEF>(selectSubtypeMapQuery, new
            {
                Id = id
            });

            return Ok(productSubtypeMap != null);
        }

        [HttpPost("client-orders/generate")]
        public async Task GenerateTestClientOrders(int count, Guid sessionId)
        {
            await _mediator.Send(new GenerateTestClientOrdersCommand(count, sessionId));
        }
    }
}
