using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using MessageBus.Rates.Import;

namespace LegacySql.Consumers.Commands.Rates.SaveErpRate
{
    public class SaveErpRateCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpRateDto>>
    {
        private readonly IDbConnection _db;

        public SaveErpRateCommandHandler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpRateDto> command, CancellationToken cancellationToken)
        {
            var sqlQuery = @"update [dbo].[Rates] set [rate]=@Value where [rateID]=@Id";
            var res = await _db.ExecuteAsync(sqlQuery, new
            {
                Id = command.Value.Code,
                command.Value.Value,
            });

            if (res == 0)
            {
                throw new KeyNotFoundException($"Id курса: {command.Value.Code} не найден");
            }

            return new Unit();
        }
    }
}
