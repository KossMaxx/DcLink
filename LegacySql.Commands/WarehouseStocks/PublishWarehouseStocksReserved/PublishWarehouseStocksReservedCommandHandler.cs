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

namespace LegacySql.Commands.WarehouseStocks.PublishWarehouseStocksReserved
{
    public class PublishWarehouseStocksReservedCommandHandler : ManagedCommandHandler<PublishWarehouseStocksReservedCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyWarehouseStockRepository _warehouseStockRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishWarehouseStocksReservedCommandHandler(
            ILastChangedDateRepository lastChangedDateRepository,
            ILegacyWarehouseStockRepository warehouseStockRepository,
            IBus bus,
            ILogger<PublishWarehouseStocksReservedCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _lastChangedDateRepository = lastChangedDateRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _bus = bus;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishWarehouseStocksReservedCommand command, CancellationToken cancellationToken)
        {
                await Publish(cancellationToken);
        }

        private async Task Publish(CancellationToken cancellationToken)
        {
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ProductStockReserved));

            await foreach (var stock in _warehouseStockRepository.GetChangedReservedWarehouseStocksAsync(lastChangedDate, null, cancellationToken))
            {
                if (stock.IsMappingsFull())
                {
                    var stockDto = MapToDto(stock);
                    var message = _messageFactory.CreateNewEntityMessage<AddWarehouseStockReservedMessage, ProductStockReservedDto>(stockDto);
                    await _bus.Publish(message, cancellationToken);
                }
            }

            //await _lastChangedDateRepository.SetAsync(typeof(ProductStockReserved), DateTime.Now);
        }

        private ProductStockReservedDto MapToDto(ProductStockReserved stock)
        {
            return new ProductStockReservedDto
            {
                ProductId = stock.ProductId.ExternalId.Value,
                WarehouseStocks = stock.WarehouseStocks.Select(e=>new WarehouseStockDto
                {
                    WarehouseId = e.WarehouseId.ExternalId.Value,
                    Quantity = e.Quantity
                }),
                //CompanyStocks = stock.CompanyStocks.Select(e=>new CompanyStockDto
                //{
                //    CompanyOkpo = e.CompanyOkpo,
                //    Quantity = e.Quantity
                //}),
                SupplierId = stock.SupplierId?.ExternalId,
                CashPrice = stock.CashPrice,
                CashlessPrice = stock.CashlessPrice
            };
        }
    }
}