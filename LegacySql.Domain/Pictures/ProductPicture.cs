using System;

namespace LegacySql.Domain.Pictures
{
    public class ProductPicture
    {
        public int Id { get; }
        public string Url { get; }
        public DateTime Date { get; }

        public ProductPicture(int id, string url, DateTime date)
        {
            Id = id;
            Url = url;
            Date = date;
        }
    }
}
