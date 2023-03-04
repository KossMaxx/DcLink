using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacySql.Queries.Diagnostics
{
    public class TemporaryMappingStatisticDto
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public int ItemCount => Items.Count();
        public IEnumerable<TemporaryMappingDto> Items { get; set; }
    }
}
