using System.Threading.Tasks;
using LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroupById;
using LegacySql.Commands.ProductTypeCategoryGroups.PublishProductTypeCategoryGroups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-type-category-groups")]
    public class ProductTypeCategoryGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductTypeCategoryGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishProductTypeCategoryGroupsCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish/{groupId}")]
        public async Task<ActionResult> PublishById(int groupId)
        {
            var command = new PublishProductTypeCategoryGroupByIdCommand(groupId);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
