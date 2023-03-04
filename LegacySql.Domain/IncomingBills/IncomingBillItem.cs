using LegacySql.Domain.Shared;

namespace LegacySql.Domain.IncomingBills
{
    public class IncomingBillItem
    {
        public decimal PriceUAH { get; }
        public int Quantity { get; }
        public IdMap NomenclatureId { get; }

        public IncomingBillItem(decimal priceUAH, int quantity, IdMap nomenclatureId)
        {
            PriceUAH = priceUAH;
            Quantity = quantity;
            NomenclatureId = nomenclatureId;
        }
    }
}
