using System;
using MediatR;

namespace LegacySql.Commands.SupplierPrices.PublishSupplierPrices
{
    public class PublishSupplierPriceCommand : IRequest
    {
        public PublishSupplierPriceCommand(int? productId = null, DateTime? date = null)
        {
            ProductId = productId;
            Date = date;
        }

        public int? ProductId { get; }
        public DateTime? Date { get; }
    }
}
