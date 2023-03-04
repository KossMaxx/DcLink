using LegacySql.Queries.SerialNumbers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/serial-numbers")]
    public class SerialNumberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SerialNumberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<SerialNumberDto>> Get(string serialNumber, Guid? clientId)
        {
            var query = new GetSerialNumberQuery(serialNumber, clientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
