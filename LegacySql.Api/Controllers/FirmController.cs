using LegacySql.Commands.Firms.PublishFirms;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/firms")]
    public class FirmController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FirmController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishFirmsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
