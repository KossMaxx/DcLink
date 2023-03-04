namespace LegacySql.Legacy.Data.Products
{
    internal class ProductDescriptionData
    {
        public int ProductId { get; set; }
        public Descriptions ProductDescriptions { get; set; }

        internal class Descriptions
        {
            public string DescriptionRu { get; set; }
            public string DescriptionUa { get; set; }
        }
    }
}
