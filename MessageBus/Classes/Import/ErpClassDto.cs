using System;
using System.Collections.Generic;

namespace MessageBus.Classes.Import
{
    public class ErpClassDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<Guid> ProductTypes { get; set; }

    }
}
