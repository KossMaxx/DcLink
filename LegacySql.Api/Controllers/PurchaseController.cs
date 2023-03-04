using System.Threading.Tasks;
using LegacySql.Commands.Purchases.PublishOpenPurchases;
using LegacySql.Commands.Purchases.PublishPurchases;
using LegacySql.Domain.Shared;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/purchases")]
    public class PurchaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchaseController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishPurchasesCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish/open")]
        public async Task<ActionResult> PublishOpenPurchases()
        {
            await _mediator.Send(new PublishOpenPurchasesCommand());
            return Ok();
        }

        [HttpGet("not-full-map-info")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetNewClients(int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetNotFullMappingInfoQuery(MappingTypes.Purchase, page, pageSize)));
        }
    }
}
