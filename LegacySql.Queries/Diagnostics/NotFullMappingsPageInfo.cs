using System.Collections.Generic;

namespace LegacySql.Queries.Diagnostics
{
    public class NotFullMappingsPageInfo
    {
        public int TotalItems { get; }
        public int CurrentPage { get; }
        public int ItemsPerPage { get; }
        public IEnumerable<NotFulMappingStatisticDto> Result { get; }

        public NotFullMappingsPageInfo(int page, int pageSize, int totalItems, IEnumerable<NotFulMappingStatisticDto> result)
        {
            CurrentPage = page;
            Result = result;
            ItemsPerPage = pageSize;
            TotalItems = totalItems;
        }
    }
}
