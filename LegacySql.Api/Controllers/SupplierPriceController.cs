using System;
using System.Threading.Tasks;
using LegacySql.Api.Infrastructure;
using LegacySql.Commands.SupplierPrices.GetInitialSipplierPrices;
using LegacySql.Commands.SupplierPrices.PublishSupplierPrices;
using LegacySql.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/supplier-prices")]
    public class SupplierPriceController : ControllerBase
    {
        private IMediator _mediator;
        public SupplierPriceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? productId, DateTime? date)
        {
            var command = new PublishSupplierPriceCommand(productId, date);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("publish-initial")]
        public async Task<ActionResult> PublishInitial(int? productId)
        {
            await _mediator.Send(new GetInitialSupplierPricesCommand(productId));
            return Ok();
        }
    }
}
