using LegacySql.Domain.Shared;

namespace LegacySql.Domain.WarehouseStock
{
    public class CompanyStock
    {
        public CompanyStock(string companyOkpo, int quantity)
        {
            Quantity = quantity;
            CompanyOkpo = companyOkpo;
        }

        public string CompanyOkpo { get; }
        public int Quantity { get; }
    }
}
