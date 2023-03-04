using System;
using System.Collections.Generic;

namespace MessageBus.ProductTypes.Import
{
    public class ProductTypeErpDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool Web { get; set; }
        public string TypeNameUkr { get; set; }
        public Guid? MainId { get; set; }
        public IEnumerable<ProductTypeCategoryErpDto> Categories { get; set; }
    }
}
