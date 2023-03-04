using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Commands.ClientOrders.PublishClientOrder;
using LegacySql.Commands.ClientOrders.PublishClientOrderArchival;
using LegacySql.Commands.ClientOrders.PublishClientOrderById;
using LegacySql.Domain.Shared;
using LegacySql.Queries.ClientOrders.GetNotPaidClientOrderIds;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LegacySql.Api.Controllers
{
    [ApiController]
    [Route("api/client-orders")]
    public class ClientOrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> Publish(int? id)
        {
            var command = new PublishClientOrderCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("archival/publish")]
        public async Task<ActionResult> ArchivalPublish()
        {
            var command = new PublishClientOrderArchivalCommand();
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("not-full-map-info")]
        public async Task<ActionResult<NotFullMappingsPageInfo>> GetNewClients(int page = 1, int pageSize = 1000)
        {
            return Ok(await _mediator.Send(new GetNotFullMappingInfoQuery(MappingTypes.ClientOrder, page, pageSize)));
        }

        [HttpGet("not-paid")]
        public async Task<ActionResult<IEnumerable<int>>> GetNotPaidClientOrders(int clientId)
        {
            var query = new GetNotPaidClientOrderIdsQuery(clientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("publish/by-id")]
        public async Task<ActionResult> PublishClientOrder(int ordersId)
        {
            var query = new PublishClientOrderByIdCommand(ordersId);
            await _mediator.Send(query);
            return Ok();
        }
    }
}
