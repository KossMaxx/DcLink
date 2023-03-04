namespace LegacySql.Domain.Countries
{
    public class CountryIsoCode
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public CountryIsoCode(int id, string code)
        {
            Id = id;
            Code = code;
        }
    }
}
