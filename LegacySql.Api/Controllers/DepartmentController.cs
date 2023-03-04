using System.Threading.Tasks;
using LegacySql.Commands.Departments.PublishDepartments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishDepartmentsCommand();
            await _mediator.Send(command);
            return Ok();
        }
    }
}
