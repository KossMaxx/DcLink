using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ReconciliationActs;
using MediatR;
using MessageBus.ReconciliationActs.Import;

namespace LegacySql.Consumers.Commands.ReconciliationActs.SaveErpReconciliationAct
{
    public class SaveErpReconciliationActCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpReconciliationActDto>>
    {
        private readonly IDbConnection _db;
        private readonly IReconciliationActMapRepository _reconciliationActMapRepository;

        public SaveErpReconciliationActCommandHandler(IDbConnection db, IReconciliationActMapRepository reconciliationActMapRepository)
        {
            _db = db;
            _reconciliationActMapRepository = reconciliationActMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpReconciliationActDto> command, CancellationToken cancellationToken)
        {
            var reconciliationAct = command.Value;
            var reconciliationActMapping = await _reconciliationActMapRepository.GetByErpAsync(reconciliationAct.Id);
            if (reconciliationActMapping == null)
            {
                throw new ArgumentException($"Акт сверки c id:{reconciliationAct.Id} не найден\n");
            }
            else
            {
                await Update(reconciliationActMapping.LegacyId);
            }

            return new Unit();
        }

        private async Task Update(int legacyId)
        {
            var updateQuery = @"update [dbo].[BalanceLog] 
                                set   [f]=1
                                where [id]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = legacyId,
            });
        }
    }
}