namespace LegacySql.Domain.Shared
{
    public class Reference
    {
        public int Id { get; }
        public string Title { get; }

        public Reference(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
