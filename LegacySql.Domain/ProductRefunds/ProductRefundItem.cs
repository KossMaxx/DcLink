using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductRefunds
{
    public class ProductRefundItem
    {
        public int? Quantity { get; set; }
        public IdMap ProductId { get; set; }
        public decimal Price { get; set; }

        public ProductRefundItem(int? quantity, IdMap productId, decimal price)
        {
            Quantity = quantity;
            ProductId = productId;
            Price = price;
        }
    }
}
