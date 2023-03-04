using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacySql.Queries.Diagnostics
{
    public class ErpNotFulMappingStatisticDto
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public int ItemCount => Items.Count();
        public IEnumerable<ErpNotFulMappingStatisticItem> Items { get; set; }
    }
    public class ErpNotFulMappingStatisticItem
    {
        public Guid Id { get; set; }
        public string Date { get; set; }
        public IEnumerable<string> Reasons { get; set; }
    }
}
