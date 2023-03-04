namespace MessageBus.ProductTypes.Export
{
    public class ProductTypeDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsGroupe { get; set; }
        public bool Web { get; set; }
        public string TypeNameUkr { get; set; }
        public int? MainId { get; set; }
    }
}
