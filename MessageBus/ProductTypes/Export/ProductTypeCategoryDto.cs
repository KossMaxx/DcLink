using MessageBus.ProductTypeCategoryGroups.Export;

namespace MessageBus.ProductTypes.Export
{ 
    public class ProductTypeCategoryDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public bool Web { get; set; }
        public bool Web2 { get; set; }
        public bool PriceTag { get; set; }
        public ProductTypeCategoryGroupDto Group { get; set; }
    }
}
