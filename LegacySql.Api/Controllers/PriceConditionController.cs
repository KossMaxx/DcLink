using System.Threading.Tasks;
using LegacySql.Commands.PriceConditions.PublishPriceConditions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/price-conditions")]
    public class PriceConditionController : ControllerBase
    {
        private IMediator _mediator;
        public PriceConditionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishPriceConditionsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
