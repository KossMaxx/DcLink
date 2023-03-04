using LegacySql.Commands.Rates.PublishRates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/rates")]
    public class RateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishChangedProducts()
        {
            var command = new PublishRateCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
