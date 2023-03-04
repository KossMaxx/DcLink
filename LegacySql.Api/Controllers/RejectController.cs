using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LegacySql.Commands.Rejects.PublishRejects;
using MediatR;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/rejects")]
    public class RejectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RejectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishChanged(int? id)
        {
            var command = new PublishRejectsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish/open")]
        public async Task<ActionResult> PublishRejects()
        {
            var command = new PublishRejectsCommand(null, true);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
