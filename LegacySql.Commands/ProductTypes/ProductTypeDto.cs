using System.Collections.Generic;

namespace LegacySql.Commands.ProductTypes
{
    public class ProductTypeDto
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsGroupe { get; set; }
        public int? MainId { get; set; }
        public bool Web { get; set; }
        public string TypeNameUkr { get; set; }
        public IEnumerable<ProductTypeCategoryDto> Categories { get; set; }
    }
}
