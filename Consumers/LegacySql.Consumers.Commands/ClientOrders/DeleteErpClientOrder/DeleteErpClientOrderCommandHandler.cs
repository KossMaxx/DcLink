using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.ClientOrders.DeleteErpClientOrder
{
    public class DeleteErpClientOrderCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClientOrderDeleteIdentifierDto>>
    {
        private readonly IDbConnection _db;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private List<ExternalMap> _clientOrderMapping;

        public DeleteErpClientOrderCommandHandler(IDbConnection db, 
            IErpChangedRepository erpChangedRepository, 
            IClientOrderMapRepository clientOrderMapRepository)
        {
            _db = db;
            _erpChangedRepository = erpChangedRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClientOrderDeleteIdentifierDto> command, CancellationToken cancellationToken)
        {
            _clientOrderMapping = (await _clientOrderMapRepository.GetRangeByErpAsync(command.Value.Id)).ToList();
            if (!_clientOrderMapping.Any())
            {
                throw new ArgumentException($"Маппинг заказа id: {command.Value.Id} не найден");
            }

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                foreach (var orderMapping in _clientOrderMapping)
                {
                    await DeleteOrderRelations(orderMapping, transaction);
                    await UpdateClientOrder(orderMapping, transaction);
                    await SaveInErpChange(orderMapping, transaction);
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.Close();
            }

            return new Unit();
        }

        private async Task UpdateClientOrder(ExternalMap orderMapping, IDbTransaction transaction)
        {
            var updateSqlQuery = @"update [dbo].[РН] set [ф]=1 where НомерПН=@OrderId";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                OrderId = orderMapping.LegacyId
            }, transaction);
        }

        private async Task DeleteOrderRelations(ExternalMap orderMapping, IDbTransaction transaction)
        {
            var selectOperationIdsSqlQuery = @"select [КодОперации] as OperationId from [dbo].[Расход] 
                                               where НомерПН=@OrderId";
            var operationIds = (await _db.QueryAsync<int>(selectOperationIdsSqlQuery, new
            {
                OrderId = orderMapping.LegacyId
            }, transaction)).ToList();

            if (operationIds.Any())
            {
                var operationIdsString = string.Join(",", operationIds.Select(g => $"'{g}'"));
                var deleteSerialNumbersSqlQuery = $@"delete from [dbo].[rushod] 
                                                where Num in ({operationIdsString})";
                await _db.ExecuteAsync(deleteSerialNumbersSqlQuery, null, transaction);

                var deleteOrderItemsSqlQuery = $@"delete from [dbo].[Расход] 
                                            where [КодОперации] in ({operationIdsString})";
                await _db.ExecuteAsync(deleteOrderItemsSqlQuery, null, transaction);
            }
        }

        private async Task SaveInErpChange(ExternalMap orderMapping, IDbTransaction transaction)
        {
            var selectOrderChangedDateQuery = @"select [modified_at] from [dbo].[РН]
                                                where [НомерПН]=@ClientOrderId";
            var orderChangedDate = await _db.QueryFirstOrDefaultAsync<DateTime>(selectOrderChangedDateQuery, new
            {
                ClientOrderId = orderMapping.LegacyId
            }, transaction);

            await _erpChangedRepository.Save(
                orderMapping.LegacyId,
                orderChangedDate,
                typeof(ClientOrder).Name
            );
        }
    }
}
