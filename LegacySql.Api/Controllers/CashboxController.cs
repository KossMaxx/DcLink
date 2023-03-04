using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Api.Models;
using LegacySql.Commands.Cashboxes.RemoveCashboxHandMapping;
using LegacySql.Commands.Cashboxes.SetCashboxHandMapping;
using LegacySql.Commands.CashboxPayments.CarryingOutCashboxApplicationPayments;
using LegacySql.Commands.CashboxPayments.CashboxApplicationPaymentPublish;
using LegacySql.Commands.CashboxPayments.CashboxPaymentPublish;
using LegacySql.Queries.Cashboxes.GetLegacyData;
using LegacySql.Queries.Cashboxes.GetMappings;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/cashboxes")]
    public class CashboxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CashboxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("payments/publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishCashboxPaymentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
        
        [HttpPost("application-payments/publish")]
        public async Task<ActionResult> ApplicationPublish(int? id)
        {
            var command = new PublishCashboxApplicationPaymentCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("application-payments/carrying-out/{id}")]
        public async Task<ActionResult> CarryingOutApplication(Guid id, CashboxApplicationPaymentCarryingInfo info)
        {
            var command = new CarryingOutCashboxApplicationPaymentCommand(id, info.IncomePaymentId, info.OutPaymentId, info.UserId, info.Date, info.HeldIn);
            await _mediator.Send(command);
            return Ok();
        }

        #region IMappingApi

        [HttpGet("mappings")]
        public async Task<ActionResult<IEnumerable<MappingDto>>> GetMappings()
        {
            return Ok(await _mediator.Send(new GetCashboxMappingsQuery()));
        }

        [HttpGet("references")]
        public async Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search)
        {
            return Ok(await _mediator.Send(new GetCashboxLegacyDataQuery(search)));
        }

        [HttpPost("mapping")]
        public async Task<ActionResult> Map(EntityMapping mapping)
        {
            return Ok(await _mediator.Send(new SetCashboxHandMappingCommand(mapping.InnerId, mapping.ErpGuid)));
        }

        [HttpDelete("mapping/{id}")]
        public async Task<ActionResult> UnmapByLegacyId(int id, Guid? erpId = null)
        {
            return Ok(await _mediator.Send(new RemoveCashboxHandMappingCommand(erpId, id)));
        }

        #endregion
    }
}
