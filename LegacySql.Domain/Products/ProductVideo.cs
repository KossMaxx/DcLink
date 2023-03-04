using System;

namespace LegacySql.Domain.Products
{
    public class ProductVideo
    {
        public int Id { get; }
        public string Url { get; }
        public DateTime Date { get; }

        public ProductVideo(int id, string url, DateTime date)
        {
            Id = id;
            Url = url;
            Date = date;
        }
    }
}
