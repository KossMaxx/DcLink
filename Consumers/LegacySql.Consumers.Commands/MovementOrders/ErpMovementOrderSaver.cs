using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.MovementOrders;
using LegacySql.Domain.Shared;
using MessageBus.MovementOrders.Import;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.MovementOrders
{
    public class ErpMovementOrderSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IMovementOrderMapRepository _movementOrderMapRepository;
        private ErpMovementOrderDto _order;
        private ExternalMap _orderMapping;
        private ExternalMap _senderMapping;
        private ExternalMap _recipientMapping;

        public ErpMovementOrderSaver(IDbConnection db, IClientMapRepository clientMapRepository, IMovementOrderMapRepository movementOrderMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _movementOrderMapRepository = movementOrderMapRepository;
        }

        internal void InitErpObject(ErpMovementOrderDto order, ExternalMap orderMapping)
        {
            _order = order;
            _orderMapping = orderMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _senderMapping = await _clientMapRepository.GetByErpAsync(_order.SenderId);
            if (_senderMapping == null)
            {
                why.Append($"Маппинг отправителя (SenderId) id:{_order.SenderId} не найден\n");
            }

            _recipientMapping = await _clientMapRepository.GetByErpAsync(_order.RecipientId);
            if (_recipientMapping == null)
            {
                why.Append($"Маппинг отправителя (RecipientId) id:{_order.RecipientId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Save(Guid messageId)
        {
            if (_orderMapping == null)
            {
                await Create(messageId);
            }
            else
            {
                await Update();
            }
        }

        private async Task Create(Guid messageId)
        {
            var insertSqlQuery = @"insert into [dbo].[BalanceMove] 
                                  ([amount],[data],[sozdal],[klientID1],[klientID2],[prim])
                                  values (@Amount,@Date,@Username,@SenderId,@RecipientId,@Notes)
                                  select cast(SCOPE_IDENTITY() as int)";
            var newOrderId = await _db.QueryFirstOrDefaultAsync<int>(insertSqlQuery, new
            {
                Amount = _order.Amount,
                Date = _order.Date,
                Username = _order.Username,
                SenderId = _senderMapping.LegacyId,
                RecipientId = _recipientMapping.LegacyId,
                Notes = _order.Notes
            });

            await _movementOrderMapRepository.SaveAsync(new ExternalMap(messageId, newOrderId, _order.Id));
        }

        private async Task Update()
        {
            var updateSqlQuery = @"update [dbo].[BalanceMove] set
                                  [amount]=@Amount,
                                  [data]=@Date,
                                  [sozdal]=@Username,
                                  [klientID1]=@SenderId,
                                  [klientID2]=@RecipientId,
                                  [prim]=@Notes
                                  where [bmID]=@Id";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id = _orderMapping.LegacyId,
                Amount = _order.Amount,
                Date = _order.Date,
                Username = _order.Username,
                SenderId = _senderMapping.LegacyId,
                RecipientId = _recipientMapping.LegacyId,
                Notes = _order.Notes
            });
        }
    }
}
