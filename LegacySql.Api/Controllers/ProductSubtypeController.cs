using System.Threading.Tasks;
using LegacySql.Commands.ProductSubtypes.PublishProductSubtypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-subtypes")]
    public class ProductSubtypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductSubtypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishProductSubtypes(int? id)
        {
            var command = new PublishProductSubtypesCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
