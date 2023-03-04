using System;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Cashboxes.RemoveCashboxHandMapping;
using LegacySql.Commands.Cashboxes.SetCashboxHandMapping;
using LegacySql.Commands.MarketSegments.PublishMarketSegment;
using LegacySql.Queries.Cashboxes.GetLegacyData;
using LegacySql.Queries.Cashboxes.GetMappings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/market-segments")]
    public class MarketSegmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MarketSegmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishMarketSegmentCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
