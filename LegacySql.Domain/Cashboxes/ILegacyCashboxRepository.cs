using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Cashboxes
{
    public interface ILegacyCashboxRepository
    {
        Task<Cashbox> Get(int id, CancellationToken cancellationToken);
    }
}
