using System.Linq;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Commands.ExecutingJobs.ClearExecutingJobsCommand
{
    class ClearExecutingJobsCommandHandler : RequestHandler<ClearExecutingJobsCommand>
    {
        private readonly AppDbContext _appDb;

        public ClearExecutingJobsCommandHandler(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        protected override async void Handle(ClearExecutingJobsCommand command)
        {
            await _appDb.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ExecutingJobs\"");
        }
    }
}