using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Domain.ErpChanged
{
    public interface IErpChangedRepository
    {
        Task Save(int legacyId, DateTime? date, string type);
        Task<IEnumerable<ErpChanged>> GetAll(string type);
        Task Delete(int legacyId, string type);
    }
}
