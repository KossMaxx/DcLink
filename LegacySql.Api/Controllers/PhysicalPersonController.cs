using System.Threading.Tasks;
using LegacySql.Api.Infrastructure;
using LegacySql.Commands.PhysicalPersons.PublishPhysicalPersons;
using LegacySql.Commands.Products.PublishProducts;
using LegacySql.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/physical-persons")]
    public class PhysicalPersonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PhysicalPersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishPhysicalPersonsCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
