using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Warehouses.MapWarehouse;
using LegacySql.Commands.Warehouses.UnmapWarehouse;
using LegacySql.Queries.Shared;
using LegacySql.Queries.Warehouses.GetMappings;
using LegacySql.Queries.Warehouses.GetReferences;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehouseController : ControllerBase, IMappingApi
    {
        private readonly IMediator _mediator;

        public WarehouseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region MappingApi

        [HttpGet("mappings")]
        public async Task<ActionResult<IEnumerable<MappingDto>>> GetMappings()
        {
            return Ok(await _mediator.Send(new GetWarehouseMappingsQuery()));
        }

        [HttpGet("references")]
        public async Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search)
        {
            return Ok(await _mediator.Send(new GetWarehouseLegacyReferencesQuery(search)));
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> Map(EntityMapping mapping)
        {
            return Ok(await _mediator.Send(new MapWarehouseCommand(mapping.InnerId, mapping.ErpGuid)));
        }

        [HttpDelete("mapping/{erpId}")]
        public async Task<ActionResult> Unmap(Guid erpId)
        {
            return Ok(await _mediator.Send(new UnmapWarehouseCommand(erpId)));
        }

        #endregion
    }
}
