using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Queries.Products
{
    public class ProductStatusDto
    {
        public int InnerId { get; set; }
        public Guid ErpGuid { get; set; }
        public bool ExistInSql { get; set; }
        public string MappingStatus { get; set; }
        public bool HasMappingErrors { get; set; }
    }
}
