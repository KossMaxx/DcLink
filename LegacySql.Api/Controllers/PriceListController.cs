using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Domain.SellingPrices;
using LegacySql.Queries.PriceLists;
using LegacySql.Queries.PriceLists.Get;
using LegacySql.Queries.PriceLists.GetIndividual;
using LegacySql.Queries.PriceLists.GetUnregistered;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/prices")]
    public class PriceListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PriceListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceListItemDto>>> Get([FromQuery] GetPriceListRequest request)
        {
            var query = new GetPriceListQuery
            {
                PriceColumn = new SellingPriceColumn(request.ColumnId),
                ProductTypeIds = request.ProductTypeIds,
                ProductManagerId = request.ProductManagerId,
                ManufacturerId = request.ManufacturerId,
            };

            return Ok(await _mediator.Send(query));
        }

        [HttpGet]
        [Route("individual")]
        public async Task<ActionResult<IEnumerable<IndividualPriceListItemDto>>> GetIndividualPrice([FromQuery] GetIndividualPriceListRequest request)
        {
            var query = new GetIndividualPriceListQuery
            {
                ClientId = request.ClientId,
                ProductTypeId = request.ProductTypeId,
                ManufacturerId = request.ManufacturerId,
                ProductManagerId = request.ProductManagerId
            };

            return Ok(await _mediator.Send(query));
        }

        [HttpGet]
        [Route("unregistered-product-prices")]
        public async Task<ActionResult<IEnumerable<UnregisteredProductPriceDto>>> GetUnregisteredProductPrices([FromQuery] GetUnregisteredProductPricesRequest request)
        {
            return Ok(await _mediator.Send(new GetUnregisteredProductPricesQuery
            {
                VendorCode = request.VendorCode,
                ClientTitle = request.ClientTitle
            }));
        }
    }
}