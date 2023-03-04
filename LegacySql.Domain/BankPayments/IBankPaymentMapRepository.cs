using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Domain.BankPayments
{
    public interface IBankPaymentMapRepository : IEntityMapRepository
    {
        Task<IEnumerable<BankPaymentMap>> GetRangeByErpIdAsync(Guid id);
        Task SaveAsync(BankPaymentMap newMap, Guid? id = null);
    }
}
