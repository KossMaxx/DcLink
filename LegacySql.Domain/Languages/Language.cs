namespace LegacySql.Domain.Languages
{
    public class Language
    {
        public int Id { get; private set; }
        public string Title { get; private set; }

        public Language(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
