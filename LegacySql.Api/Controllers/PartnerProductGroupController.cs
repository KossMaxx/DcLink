using LegacySql.Commands.PartnerProductGroups.PublishProductGroups;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/partner-product-groups")]
    public class PartnerProductGroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PartnerProductGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishPartnerProductGroups()
        {
            var command = new PublishPartnerProductGroupsCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
