using System.Threading.Tasks;
using LegacySql.Commands.ProductRefunds.PublishProductRefunds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-refunds")]
    public class ProductRefundController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductRefundController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        
        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishProductRefundsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
