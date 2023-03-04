using LegacySql.Commands.SegmentationTurnovers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/segmentation-turnovers")]
    public class SegmentationTurnoverController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SegmentationTurnoverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishSegmentationTurnovers()
        {
            var command = new PublishSegmentationTurnoversCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
