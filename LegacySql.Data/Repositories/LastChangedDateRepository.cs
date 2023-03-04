using System;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class LastChangedDateRepository : ILastChangedDateRepository
    {
        private readonly AppDbContext _db;

        public LastChangedDateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DateTime?> GetAsync(Type entityType)
        {
            var dateEf = await _db.LastChangedDates.FirstOrDefaultAsync(d => d.EntityType == entityType.Name);
            return dateEf?.Date;
        }

        public async Task SetAsync(Type entityType, DateTime lastDate)
        {
            var dateEf = await _db.LastChangedDates.FirstOrDefaultAsync(d => d.EntityType == entityType.Name);

            if (dateEf == null)
            {
                var newDate = new LastChangedDateEF
                {
                    EntityType = entityType.Name,
                    Date = lastDate
                };
                await _db.LastChangedDates.AddAsync(newDate);
            }
            else
            {
                dateEf.Date = lastDate;
            }

            await _db.SaveChangesAsync();
        }
    }
}
