using System;
using System.Threading.Tasks;

namespace LegacySql.Domain.Cashboxes
{
    public interface ICashboxMapRepository
    {
        Task SaveAsync(CashboxMap newMap, Guid? id = null);
        Task<CashboxMap> GetByLegacyAsync(int legacyId);
        Task RemoveByErpAsync(Guid erpId);
        Task<CashboxMap> GetByErpAsync(Guid erpId);
        Task RemoveByLegacyAsync(int legacyId);
    }
}
