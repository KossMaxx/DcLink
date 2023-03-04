using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Purchases
{
    public class PurchaseItem
    {
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public IdMap ProductId { get; set; }

        public PurchaseItem(decimal? price, int? quantity, IdMap productId)
        {
            Price = price;
            Quantity = quantity;
            ProductId = productId;
        }
    }
}
