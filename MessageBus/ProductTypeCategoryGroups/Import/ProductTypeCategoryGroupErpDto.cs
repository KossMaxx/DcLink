using System;
using System.Collections.Generic;

namespace MessageBus.ProductTypeCategoryGroups.Import
{
    public class ProductTypeCategoryGroupErpDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public int Sort { get; set; }
    }
}
