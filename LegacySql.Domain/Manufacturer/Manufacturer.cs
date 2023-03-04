namespace LegacySql.Domain.Manufacturer
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public Manufacturer()
        {

        }

        public Manufacturer(int id, string title, string url)
        {
            Id = id;
            Title = title;
            Url = url;
        }
    }
}
