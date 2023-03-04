using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LegacySql.Commands.ReconciliationActs.PublishReconciliationActs;
using MediatR;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/reconciliation-acts")]
    public class ReconciliationActController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReconciliationActController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishChanged(int? id)
        {
            var command = new PublishReconciliationActsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
