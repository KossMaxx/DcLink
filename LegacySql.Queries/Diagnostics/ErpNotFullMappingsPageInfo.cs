using System.Collections.Generic;

namespace LegacySql.Queries.Diagnostics
{
    public class ErpNotFullMappingsPageInfo
    {
        public int TotalItems { get; }
        public int CurrentPage { get; }
        public int ItemsPerPage { get; }
        public IEnumerable<ErpNotFulMappingStatisticDto> Result { get; }

        public ErpNotFullMappingsPageInfo(int page, int pageSize, int totalItems, IEnumerable<ErpNotFulMappingStatisticDto> result)
        {
            CurrentPage = page;
            Result = result;
            ItemsPerPage = pageSize;
            TotalItems = totalItems;
        }
    }
}
