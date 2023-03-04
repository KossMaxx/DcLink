using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Domain.Shared;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.Diagnostics.GetActualProductsIds;
using LegacySql.Queries.Diagnostics.GetErpNotFullMappingCounts;
using LegacySql.Queries.Diagnostics.GetErpNotFullMappingInfo;
using LegacySql.Queries.Diagnostics.GetNotFullMappingCounts;
using LegacySql.Queries.Diagnostics.GetProductMappingsAccordance;
using LegacySql.Queries.Diagnostics.GetProductsWithoutMapping;
using LegacySql.Queries.Diagnostics.GetTemporaryMappingCounts;
using LegacySql.Queries.Diagnostics.GetTemporaryMappingInfo;
using LegacySql.Queries.Diagnostics.HealthCheck;
using LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/diagnostic")]
    public class DiagnosticController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DiagnosticController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("not-full-mappings-counts")]
        public async Task<ActionResult> GetNotFullMappingCounts()
        {
            return Ok(await _mediator.Send(new GetNotFullMappingCountsQuery()));
        }

        [HttpGet]
        [Route("not-full-mappings-by-type")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetNotFullMappingsByType(MappingTypes type, int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetNotFullMappingInfoQuery(type, page, pageSize)));
        }

        [HttpGet]
        [Route("temporary-mappings-counts")]
        public async Task<ActionResult> GetTemporaryMappingCounts()
        {
            return Ok(await _mediator.Send(new GetTemporaryMappingCountsQuery()));
        }

        [HttpGet]
        [Route("temporary-mappings-by-type")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetTemporaryMappingsByType(MappingTypes type, int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetTemporaryMappingInfoQuery(type, page, pageSize)));
        }

        [HttpPost]
        [Route("products-without-mapping")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetProductsWithoutMapping([FromBody]IEnumerable<Guid> data)
        {
            if (data.Count() > 1000)
            {
                throw new DataException("Слишком большой объем данных");
            }
            return Ok(await _mediator.Send(new GetProductsWithoutMappingQuery(data)));
        }

        [HttpGet]
        [Route("health-check")]
        public async Task<ActionResult> HealthCheck()
        {
            return Ok(await _mediator.Send(new HealthCheckQuery()));
        }

        [HttpGet]
        [Route("erp-not-full-mappings-counts")]
        public async Task<ActionResult> GetErpNotFullMappingCounts()
        {
            return Ok(await _mediator.Send(new GetErpNotFullMappingCountsQuery()));
        }

        [HttpGet]
        [Route("erp-not-full-mappings-by-type")]
        public async Task<ActionResult<ErpNotFullMappingsPageInfo>> GetErpNotFullMappingsByType(MappingTypes type, int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetErpNotFullMappingInfoQuery(type, page, pageSize)));
        }

        [HttpGet]
        [Route("product-mappings-accordance")]
        public async Task<ActionResult<ProductMappingsAccordanceResponse>> GetProductMappingsAccordance()
        {
            return Ok(await _mediator.Send(new GetProductMappingsAccordanceQuery()));
        }

        [HttpGet]
        [Route("actual-products-ids")]
        public async Task<ActionResult<IEnumerable<long>>> GetActualProductsIds()
        {
            return Ok(await _mediator.Send(new GetActualProductsIdsQuery()));
        }
    }
}
