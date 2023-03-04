using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.ProductTypes.MapProductType;
using LegacySql.Commands.ProductTypes.PublishProductTypeById;
using LegacySql.Commands.ProductTypes.PublishProductTypes;
using LegacySql.Commands.ProductTypes.UnmapProductType;
using LegacySql.Commands.RemoveNotAllowed.RemoveNotUsedProductType;
using LegacySql.Queries.ProductTypes.GetMappings;
using LegacySql.Queries.ProductTypes.GetProductTypeLegacyReferences;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/product-types")]
    public class ProductTypeController : ControllerBase, IMappingApi
    {
        private readonly IMediator _mediator;

        public ProductTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish()
        {
            var command = new PublishProductTypesCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish/{productTypeId}")]
        public async Task<ActionResult> PublishById(int productTypeId)
        {
            var command = new PublishProductTypeByIdCommand(productTypeId);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("remove-not-allowed-product-types")]
        public async Task<ActionResult> RemoveNotAllowedProductTypes()
        {
            var command = new RemoveNotUsedProductTypeCommand();
            await _mediator.Send(command);
            return Ok();
        }

        #region MappingApi

        [HttpGet("mappings")]
        public async Task<ActionResult<IEnumerable<MappingDto>>> GetMappings()
        {
            return Ok(await _mediator.Send(new GetProductTypeMappingsQuery()));
        }

        [HttpGet("references")]
        public async Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search)
        {
            return Ok(await _mediator.Send(new GetProductTypeLegacyReferencesQuery(search)));
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> Map(EntityMapping mapping)
        {
            return Ok(await _mediator.Send(new MapProductTypeCommand(mapping.InnerId, mapping.ErpGuid)));
        }

        [HttpDelete("mapping/{erpId}")]
        public async Task<ActionResult> Unmap(Guid erpId)
        {
            return Ok(await _mediator.Send(new UnmapProductTypeCommand(erpId)));
        }

        #endregion
    }
}
