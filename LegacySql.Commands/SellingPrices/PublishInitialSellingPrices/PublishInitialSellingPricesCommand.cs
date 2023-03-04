using MediatR;

namespace LegacySql.Commands.SellingPrices.PublishInitialSellingPrices
{
    public class PublishInitialSellingPricesCommand : IRequest
    {
        public PublishInitialSellingPricesCommand(int? productId)
        {
            ProductId = productId;
        }

        public int? ProductId { get; }
    }
}
