using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Bills
{
    public class BillItem
    {
        public IdMap NomenclatureId { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        public decimal PriceUAH { get; }
        public short Warranty { get; }

        public BillItem(
            IdMap nomenclatureId, 
            int quantity, 
            decimal? price, 
            decimal? priceUah, 
            short warranty)
        {
            NomenclatureId = nomenclatureId;
            Quantity = quantity;
            Warranty = warranty;
            Price = price ?? 0;
            PriceUAH = priceUah ?? 0;
        }
    }
}
