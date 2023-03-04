using System.Collections.Generic;
using System.Linq;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Cashboxes
{
    public class Cashbox
    {
        public IdMap Id { get; }
        public string Description { get; }

        public Cashbox(IdMap id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
