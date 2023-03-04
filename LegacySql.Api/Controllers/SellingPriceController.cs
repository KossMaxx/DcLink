using System.Threading.Tasks;
using LegacySql.Api.Infrastructure;
using LegacySql.Commands.SellingPrices.PublishInitialSellingPrices;
using LegacySql.Commands.SellingPrices.PublishSellingPrices;
using LegacySql.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/selling-prices")]
    public class SellingPriceController : ControllerBase
    {
        private IMediator _mediator;
        public SellingPriceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishSellingPriceCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish-initial")]
        public async Task<ActionResult> PublishInitial(int? id)
        {
            var command = new PublishInitialSellingPricesCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
