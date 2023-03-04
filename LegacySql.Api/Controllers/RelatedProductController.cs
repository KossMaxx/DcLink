using System.Threading.Tasks;
using LegacySql.Api.Infrastructure;
using LegacySql.Commands.RelatedProducts.PublishRelatedProducts;
using LegacySql.Domain.Shared;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.NotFullMapping;
using LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/related-products")]
    public class RelatedProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RelatedProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishRelatedProductsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("not-full-map-info")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetNewClients(int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetNotFullMappingInfoQuery(MappingTypes.RelatedProduct, page, pageSize)));
        }
    }
}
