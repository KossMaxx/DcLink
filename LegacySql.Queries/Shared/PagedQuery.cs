using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Queries.Shared
{
    public abstract class PagedQuery
    {
        protected const int MaxValuePageSize = 1000;
        protected const int MinValuePage = 1;
        public int Page { get; }
        public int PageSize { get; }

        protected PagedQuery(int page, int pageSize)
        {
            Page = (page < MinValuePage) ? MinValuePage : page;
            PageSize = pageSize > MaxValuePageSize ? MaxValuePageSize : pageSize;
        }
    }
}
