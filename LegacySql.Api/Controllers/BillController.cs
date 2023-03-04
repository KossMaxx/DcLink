using LegacySql.Commands.Bills.PublishBillOrder;
using LegacySql.Commands.Bills.PublishIncomingBills;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/bills")]
    public class BillController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BillController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("order/publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishBillOrderCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("incoming/publish")]
        public async Task<ActionResult> PublishIncomingBills(int? id)
        {
            var command = new PublishIncomingBillCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
