using System.Collections.Generic;

namespace LegacySql.Queries.Diagnostics.GetProductMappingsAccordance
{
    public class ProductMappingsAccordanceResponse
    {
        public int SqlIdsCount { get; set; }
        public int MappingsCount { get; set; }
        public int TemporaryMappingCount { get; set; }
        public int ExcessInService { get; set; }
        public int DeficitInService { get; set; }
        public int InNotFullMapping { get; set; }
    }
}
