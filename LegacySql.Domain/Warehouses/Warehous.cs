using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Warehouses
{
    public class Warehouse
    {
        public IdMap Id { get; }
        public string Description { get; }

        public Warehouse(IdMap id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
