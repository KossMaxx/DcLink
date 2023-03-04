using System.Threading.Tasks;
using LegacySql.Commands.Rates.PublishRates;
using LegacySql.Commands.SupplierCurrencyRates.PublishSupplierCurrencyRate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/supplier-currency-rates")]
    public class SupplierCurrencyRateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupplierCurrencyRateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> PublishChangedProducts(int? clientId)
        {
            var command = new PublishSupplierCurrencyRateCommand(clientId);
            await _mediator.Send(command);

            if (!clientId.HasValue)
            {
                var commandCommonRates = new PublishRateCommand();
                await _mediator.Send(commandCommonRates);
            }           

            return Ok();
        }
    }
}
