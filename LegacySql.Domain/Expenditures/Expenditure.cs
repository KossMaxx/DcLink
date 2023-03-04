namespace LegacySql.Domain.Expenditures
{
    public class Expenditure
    {
        public int Id { get; set; }
        public int NomenclatureId { get; set; }
        public int Quantity { get; set; }
        public int ClientOrderId { get; set; }
    }
}
