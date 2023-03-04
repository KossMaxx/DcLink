using LegacySql.Commands.ProductMovings.PublishProductMovings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-movings")]
    public class ProductMovingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductMovingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishProductMovingCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
