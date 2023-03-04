using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.Shared;
using LegacySql.Domain.WarehouseStock;
using MassTransit;
using MessageBus.WarehouseStocks.Export;
using MessageBus.WarehouseStocks.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.WarehouseStocks.PublishCompanyStocks
{
    public class PublishCompanyStocksCommandHandler : ManagedCommandHandler<PublishCompanyStocksCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyWarehouseStockRepository _warehouseStockRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishCompanyStocksCommandHandler(
            ILastChangedDateRepository lastChangedDateRepository,
            ILegacyWarehouseStockRepository warehouseStockRepository,
            IBus bus,
            ILogger<PublishCompanyStocksCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _lastChangedDateRepository = lastChangedDateRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _bus = bus;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishCompanyStocksCommand command, CancellationToken cancellationToken)
        {
                await Publish(cancellationToken);
        }

        private async Task Publish(CancellationToken cancellationToken)
        {
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ProductBNStock));

            await foreach (var stock in _warehouseStockRepository.GetChangedCompanyStocksAsync(lastChangedDate, null, cancellationToken))
            {
                if (stock.IsMappingsFull())
                {
                    var stockDto = MapToDto(stock);
                    var message = _messageFactory.CreateNewEntityMessage<AddCompanyStockMessage, ProductBNStockDto>(stockDto);
                    await _bus.Publish(message, cancellationToken);
                }
            }

            //await _lastChangedDateRepository.SetAsync(typeof(ProductBNStock), DateTime.Now);
        }

        private ProductBNStockDto MapToDto(ProductBNStock stock)
        {
            return new ProductBNStockDto
            {
                ProductId = stock.ProductId.ExternalId.Value,
                CompanyStocks = stock.CompanyStocks.Select(e => new CompanyStockDto
                {
                    CompanyOkpo = e.CompanyOkpo,
                    Quantity = e.Quantity
                }),
                SupplierId = stock.SupplierId?.ExternalId,
                CashPrice = stock.CashPrice,
                CashlessPrice = stock.CashlessPrice
            };
        }
    }
}