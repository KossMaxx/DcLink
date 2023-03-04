using MediatR;

namespace LegacySql.Commands.SupplierPrices.GetInitialSipplierPrices
{
    public class GetInitialSupplierPricesCommand : IRequest
    {
        public GetInitialSupplierPricesCommand(int? productId)
        {
            this.ProductId = productId;
        }

        public int? ProductId { get; }
    }
}
