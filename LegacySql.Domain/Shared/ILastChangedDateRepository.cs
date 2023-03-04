using System;
using System.Threading.Tasks;

namespace LegacySql.Domain.Shared
{
    public interface ILastChangedDateRepository
    {
        Task<DateTime?> GetAsync(Type entityType);
        Task SetAsync(Type entityType, DateTime lastDate);
    }
}
