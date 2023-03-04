using System;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Commands.Shared
{
    public class CommandsHandlerManager : ICommandsHandlerManager
    {
        private readonly AppDbContext _appDb;

        public CommandsHandlerManager(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<bool> IsExecuted(Type handlerType)
        {
            return await _appDb.ExecutingJobs.AnyAsync(j => j.JobType == handlerType.Name);
        }

        public async Task Start(Type handlerType)
        {
            if (await _appDb.ExecutingJobs.AnyAsync(j => j.JobType == handlerType.Name))
            {
                return;
            }

            await _appDb.ExecutingJobs.AddAsync(new ExecutingJobEF { JobType = handlerType.Name });
            await _appDb.SaveChangesAsync();
        }

        public async Task Finish(Type handlerType)
        {
            var job = await _appDb.ExecutingJobs.FirstOrDefaultAsync(j => j.JobType == handlerType.Name);
            if (job == null)
            {
                return;
            }

            _appDb.ExecutingJobs.Remove(job);
            await _appDb.SaveChangesAsync();
        }
    }
}
