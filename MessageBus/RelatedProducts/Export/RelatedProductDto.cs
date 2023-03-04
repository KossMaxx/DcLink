using System;

namespace MessageBus.RelatedProducts.Export
{
    public class RelatedProductDto
    {
        public Guid? MainProductId { get; set; }
        public Guid? RelatedProductId { get; set; }
    }
}
