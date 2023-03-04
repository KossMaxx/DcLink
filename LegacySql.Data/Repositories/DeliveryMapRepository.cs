using LegacySql.Data.Models;
using LegacySql.Domain.Deliveries;

namespace LegacySql.Data.Repositories
{
    public class DeliveryMapRepository : BaseMapRepository<DeliveryMapEF>, IDeliveryMapRepository
    {
        public DeliveryMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
