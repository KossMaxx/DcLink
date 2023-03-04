using LegacySql.Commands.ActivityTypes.PublishActivityTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/activity-type")]
    public class ActivityTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActivityTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishActivityTypes()
        {
            var command = new PublishActivityTypesCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
