using System.Collections.Generic;

namespace LegacySql.Commands.ProductTypes
{
    public class ProductTypeCategoryDto
    {
        public string Name { get; set; }
        public string NameUA { get; set; }
        public IEnumerable<ProductTypeCategoryParameterDto> Parameters { get; set; }
    }
}
