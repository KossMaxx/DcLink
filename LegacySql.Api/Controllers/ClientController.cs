using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Clients.ChangeClientMapping;
using LegacySql.Commands.Clients.Map;
using LegacySql.Commands.Clients.PublishClients;
using LegacySql.Commands.Clients.Unmap;
using LegacySql.Commands.RemoveNotAllowed.RemoveNotAllowedClients;
using LegacySql.Queries.Clients.GetClientMappings;
using LegacySql.Queries.Clients.GetClientReferences;
using LegacySql.Queries.Clients.IsMappingExist;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase, IMappingApi
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishClientsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("remove-not-allowed-clients")]
        public async Task<ActionResult> RemoveNotAllowedClients()
        {
            var command = new RemoveNotAllowedClientsCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("change-client-mappings")]
        public async Task<ActionResult> ChangeClientMappings(IEnumerable<ChangeMappingIds> mappings)
        {
            foreach (var mapping in mappings)
            {
                var command = new ChangeClientMappingCommand(mapping.OldId, mapping.NewId);
                await _mediator.Send(command);
            }

            return Ok();
        }

        [HttpGet("is-mapping-exist/{id}")]
        public async Task<ActionResult> IsMappingExist(Guid id)
        {
            var request = new IsMappingExistRequest(id);
            return Ok(await _mediator.Send(request));
        }

        #region MappingApi
        [HttpGet("mappings")]
        public async Task<ActionResult<IEnumerable<MappingDto>>> GetMappings()
        {
            return Ok(await _mediator.Send(new GetClientMappingsQuery()));
        }

        [HttpGet("references")]
        public async Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search)
        {
            var query = new GetClientLegacyReferencesQuery(search);
            var references = await _mediator.Send(query);
            return Ok(references);
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> Map(EntityMapping mapping)
        {
            var command = new MapClientCommand(mapping.InnerId, mapping.ErpGuid);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("mapping/{erpId}")]
        public async Task<ActionResult> Unmap(Guid erpId)
        {
            var command = new UnmapClientCommand(erpId);
            await _mediator.Send(command);
            return Ok();
        }

        #endregion
    }
}
