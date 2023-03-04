using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Employees;
using LegacySql.Commands.Employees.PublishEmployees;
using LegacySql.Queries.Employees;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController : ControllerBase, IMappingApi
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishEmployeesCommand();
            await _mediator.Send(command);
            return Ok();
        }

        #region MappingApi

        [HttpGet("mappings")]
        public async Task<ActionResult<IEnumerable<MappingDto>>> GetMappings()
        {
            var query = new GetEmployeeMappingsQuery();
            var mappings = await _mediator.Send(query);
            return Ok(mappings);
        }

        [HttpGet("references")]
        public async Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search)
        {
            var query = new GetEmployeeLegacyReferencesQuery(search);
            var references = await _mediator.Send(query);
            return Ok(references);
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> Map(EntityMapping mapping)
        {
            var command = new MapEmployeeCommand(mapping.InnerId, mapping.ErpGuid);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("mapping/{erpId}")]
        public async Task<ActionResult> Unmap(Guid erpId)
        {
            var command = new UnmapEmployeeCommand(erpId);
            await _mediator.Send(command);
            return Ok();
        }
        #endregion
    }
}
