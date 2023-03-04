namespace LegacySql.Queries.Shared
{
    public class LegacyReferenceDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public LegacyReferenceDto() { }

        public LegacyReferenceDto(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
