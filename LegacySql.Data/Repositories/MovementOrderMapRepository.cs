using LegacySql.Data.Models;
using LegacySql.Domain.MovementOrders;

namespace LegacySql.Data.Repositories
{
    public class MovementOrderMapRepository : BaseMapRepository<MovementOrderMapEF>, IMovementOrderMapRepository
    {
        public MovementOrderMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
