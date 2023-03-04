using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Warehouses
{
    public interface ILegacyWarehouseRepository
    {
        Task<Warehouse> Get(int id, CancellationToken cancellationToken);
    }
}
