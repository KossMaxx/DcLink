using System.Threading.Tasks;
using LegacySql.Api.Infrastructure;
using LegacySql.Commands.WarehouseStocks.PublishCompanyStocks;
using LegacySql.Commands.WarehouseStocks.PublishCompanyStocksReserved;
using LegacySql.Commands.WarehouseStocks.PublishWarehouseStocks;
using LegacySql.Commands.WarehouseStocks.PublishWarehouseStocksReserved;
using LegacySql.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/warehouse-stocks")]
    public class WarehouseStockController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WarehouseStockController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishWarehouseStocksCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish-reserv")]
        public async Task<ActionResult> PublishReserv()
        {
            var command = new PublishWarehouseStocksReservedCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish-company")]
        public async Task<ActionResult> PublishCompany()
        {
            var command = new PublishCompanyStocksCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish-company-reserv")]
        public async Task<ActionResult> PublishCompanyReserved()
        {
            var command = new PublishCompanyStocksReservedCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
