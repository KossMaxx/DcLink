using System;

namespace MessageBus.ProductSubtypes.Import
{
    public class ErpProductSubtypeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid ProductTypeId { get; set; }
    }
}
