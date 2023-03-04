using System;
using System.Collections.Generic;
using MessageBus.ProductTypeCategoryGroups.Import;

namespace MessageBus.ProductTypes.Import
{
    public class ProductTypeCategoryErpDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public ProductTypeCategoryGroupErpDto Group { get; set; }
        public IEnumerable<ProductTypeCategoryParameterErpDto> Parameters { get; set; }
    }
}
