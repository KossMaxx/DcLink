using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Products.ChangeWrongMapping;
using LegacySql.Commands.Products.PublishProducts;
using LegacySql.Commands.Products.SetProductMapping;
using LegacySql.Commands.RemoveNotAllowed.RemoveNotAllowedProducts;
using LegacySql.Domain.Shared;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.NotFullMapping;
using LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo;
using LegacySql.Queries.Products;
using LegacySql.Queries.Products.GetNotActualGuids;
using LegacySql.Queries.Products.GetStatusOfProduct;
using LegacySql.Queries.Products.IsProductMappingExist;
using MediatR;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishChangedProducts(int? id)
        {
            var command = new PublishProductsCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("remove-not-allowed-products")]
        public async Task<ActionResult> RemoveNotAllowedProducts()
        {
            var command = new RemoveNotAllowedProductsCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("statuses")]
        public async Task<ActionResult> GetStatusesOfProducts(IEnumerable<EntityMapping> mappings)
        {
            var result = new List<ProductStatusDto>();
            foreach (var mapping in mappings)
            {
                var command = new GetStatusOfProductQuery( mapping.InnerId, mapping.ErpGuid );
                result.Add(await _mediator.Send(command));
            }
            return Ok(result);
        }

        [HttpGet("not-full-map-info")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetNewClients(int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetNotFullMappingInfoQuery(MappingTypes.Product, page, pageSize)));
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> SetMapping(EntityMapping mapping)
        {
            return Ok(await _mediator.Send(new SetProductMappingCommand(mapping.InnerId, mapping.ErpGuid)));
        }

        [HttpGet("is-mapping-exist/{id}")]
        public async Task<ActionResult> IsMappingExist(Guid id)
        {
            var request = new IsProductMappingExistQuery(id);
            return Ok(await _mediator.Send(request));
        }

        [HttpGet("not-actual-guids")]
        public async Task<ActionResult> GetActualGuids()
        {
            return Ok(await _mediator.Send(new GetNotActualGuidsRequest()));
        }

        [HttpPost("change-wrong-mapping")]
        public async Task<ActionResult> ChangeWrongMapping(EntityMapping mapping)
        {
            return Ok(await _mediator.Send(new ChangeWrongMappingCommand(mapping.InnerId, mapping.ErpGuid)));
        }
    }
}
