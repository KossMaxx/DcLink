using System;
using System.Collections.Generic;

namespace MessageBus.Classes.Export
{
    public class ClassDto 
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public IEnumerable<Guid> ProductTypes { get; set; }
    }
}
