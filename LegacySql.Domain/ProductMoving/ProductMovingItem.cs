using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductMoving
{
    public class ProductMovingItem
    {
        public IdMap ProductId { get; }
        public int Amount { get; }
        public decimal Price { get; }
        public ProductMovingItem(IdMap productId, int amount, decimal price)
        {
            ProductId = productId;
            Amount = amount;
            Price = price;
        }
    }
}
