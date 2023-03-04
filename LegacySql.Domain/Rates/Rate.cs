namespace LegacySql.Domain.Rates
{
    public class Rate
    {
        public int Id { get; }
        public string Title { get; }
        public decimal Value { get; }

        public Rate(int id, string title, decimal value)
        {
            Id = id;
            Title = title;
            Value = value;
        }
    }
}
