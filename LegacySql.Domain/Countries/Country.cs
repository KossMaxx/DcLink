using LegacySql.Domain.ValueObjects;

namespace LegacySql.Domain.Countries
{
    public class Country
    {
        public int Id { get; private set; }
        public CountryIsoCode Iso { get; private set; }
        public string Title { get; private set; }
        public LanguageId LanguageId { get; private set; }

        public Country(int id, CountryIsoCode iso, string title, LanguageId languageId)
        {
            Id = id;
            Iso = iso;
            Title = title;
            LanguageId = languageId;
        }
    }
}
