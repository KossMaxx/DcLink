using LegacySql.Queries.PriceAlgorythms;
using LegacySql.Queries.PriceAlgorythms.Get;
using LegacySql.Queries.PriceAlgorythms.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Commands.PriceAlgorythm.RecalculatePrices;
using LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm;
using LegacySql.Queries.PriceAlgorythms.PreliminaryPrices;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/price-algorythms")]
    public class PriceAlgorythmController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PriceAlgorythmController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceAlgorythmReferenceDto>>> Search([FromQuery] string searchTerm)
        {
            var query = new SearchPriceAlgorythmQuery
            {
                SearchTerm = searchTerm
            };

            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PriceAlgorythmDto>> Get([FromRoute] int id)
        {
            var query = new GetPriceAlgorythmQuery
            {
                Id = id
            };

            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{id}/preliminary-prices")]
        public async Task<ActionResult<PriceAlgorythmDto>> PreliminaryPrices([FromRoute] int id)
        {
            var query = new GetPreliminaryPricesQuery
            {
                Id = id
            };

            return Ok(await _mediator.Send(query));
        }


        [HttpPost("{id}/recalculate-prices")]
        public async Task<ActionResult> RecalculatePrices([FromRoute] int id)
        {
            var command = new RecalculatePricesCommand(id);
            
            await _mediator.Send(command);
            
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] ErpPriceAlgorythmDto update)
        {
            var command = new SaveErpPriceAlgorythmCommand(update);
            
            int id = await _mediator.Send(command);
            
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ErpPriceAlgorythmDto update)
        {
            var command = new SaveErpPriceAlgorythmCommand(update, id);
            
            await _mediator.Send(command);
            
            return Ok();
        }
    }
}