using System.Threading.Tasks;
using LegacySql.Commands.ProductPriceConditions.PublishProductPriceConditions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-price-conditions")]
    public class ProductPriceConditionController : ControllerBase
    {
        private IMediator _mediator;
        public ProductPriceConditionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishProductPriceConditionsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
