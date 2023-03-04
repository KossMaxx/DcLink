using System;

namespace MessageBus.Products.Export
{
    public class ProductCategoryParameterDto
    {
        public Guid CategoryId { get; set; }
        public Guid? ParameterId { get; set; }
    }
}
