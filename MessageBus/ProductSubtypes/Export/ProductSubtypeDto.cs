using System;

namespace MessageBus.ProductSubtypes.Export
{
    public class ProductSubtypeDto
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public Guid ProductTypeId { get; set; }
    }
}
