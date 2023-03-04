using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.ProductTypeCategoryGroups.Export
{
    public class ProductTypeCategoryGroupDto
    {
        public int LegacyId { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public int Sort { get; set; }
    }
}
