using System.ComponentModel.DataAnnotations;
using LegacySql.Commands.LoggingSettings.SetHttpBodyLogging;
using LegacySql.Queries.LoggingSettings.GetLoggingSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/logging-settings")]
    public class LoggingSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoggingSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public ActionResult<LoggingSettingsDto> Get()
        {
            var query = new GetLoggingSettingsQuery();
            LoggingSettingsDto settings = _mediator.Send(query).Result;
            return Ok(settings);
        }

        [HttpPatch("http-body-logging")]
        public ActionResult SetHttpBodyLogging([Required] [FromQuery] bool httpBodyLogging)
        {
            var command = new SetHttpBodyLoggingCommand
            {
                HttpBodyLogging = httpBodyLogging,
            };
        
            _mediator.Send(command).Wait();
        
            return Ok();
        }
    }
}