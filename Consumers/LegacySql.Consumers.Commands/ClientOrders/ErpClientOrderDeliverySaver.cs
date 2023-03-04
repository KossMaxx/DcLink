using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Deliveries;
using LegacySql.Domain.Shared;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.ClientOrders
{
    public class ErpClientOrderDeliverySaver
    {
        private readonly IDbConnection _db;
        private readonly IDeliveryMapRepository _clientOrderDeliveryMapRepository;
        private ExternalMap _clientOrderMapping;
        private ErpClientOrderDeliveryDto _delivery;

        public ErpClientOrderDeliverySaver(
            IDbConnection db, 
            IDeliveryMapRepository clientOrderDeliveryMapRepository)
        {
            _db = db;
            _clientOrderDeliveryMapRepository = clientOrderDeliveryMapRepository;
        }
        public void InitErpObject(ErpClientOrderDeliveryDto delivery, ExternalMap clientOrderMapping)
        {
            _delivery = delivery;
            _clientOrderMapping = clientOrderMapping;
        }

        public MappingInfo GetMappingInfo()
        {
            var why = new StringBuilder();
            if (_clientOrderMapping == null)
            {
                why.Append($"Маппинг заказа id: {_delivery.OrderId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Save()
        {
            var clientOrderDeliveryMap = await _clientOrderDeliveryMapRepository.GetByErpAsync(_delivery.Id);
            if (clientOrderDeliveryMap == null)
            {
                await Create();
            }
            else
            {
                await Update(clientOrderDeliveryMap.LegacyId);
            }
        }

        private async Task Update(int deliveryId)
        {
            var updateDeliveryQuery = @"update [dbo].[dostavka]   
                                      set [poluchatel]=@RecipientName,
                                      [tel]=@RecipientPhone,
                                      [kuda]=@RecipientAddress,
                                      [CityRecipient]=@RecipientCityId,
                                      [email]=@RecipientEmail,
                                      [Weight]=@Weight,
                                      [Vol]=@Volume,
                                      [DeclaredPrice]=@DeclaredPrice,
                                      [PaymentMethod]=@PaymentMethod,
                                      [CashOnDelivery]=@CashOnDelivery,
                                      [CargoInvoice]=@CargoInvoice,
                                      [klientID] = (select [klientID] from [dbo].[РН] where [НомерПН] = @OrderId)
                                      where [id]=@DeliveryId";
            await _db.ExecuteAsync(updateDeliveryQuery, new
            {
                DeliveryId = deliveryId,
                _delivery.RecipientName,
                _delivery.RecipientPhone,
                _delivery.RecipientAddress,
                _delivery.RecipientCityId,
                _delivery.RecipientEmail,
                _delivery.Weight,
                _delivery.Volume,
                _delivery.DeclaredPrice,
                _delivery.PaymentMethod,
                _delivery.CashOnDelivery,
                _delivery.CargoInvoice,
                OrderId = _clientOrderMapping.LegacyId
            });
        }

        private async Task Create()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertDeliveryQuery = @"insert into [dbo].[dostavka]   
                                          ([poluchatel],
                                          [tel],
                                          [kuda],
                                          [CityRecipient],
                                          [email],
                                          [Weight],
                                          [Vol],
                                          [DeclaredPrice],
                                          [PaymentMethod],
                                          [CashOnDelivery],
                                          [CargoInvoice],
                                          [klientID])
                                          values 
                                          (@RecipientName,
                                          @RecipientPhone,
                                          @RecipientAddress,
                                          @RecipientCityId,
                                          @RecipientEmail,
                                          @Weight,
                                          @Volume,
                                          @DeclaredPrice,
                                          @PaymentMethod,
                                          @CashOnDelivery,
                                          @CargoInvoice,
                                          (select [klientID] from [dbo].[РН] where [НомерПН] = @OrderId));
                                          select cast(SCOPE_IDENTITY() as int)";
                var newDeliveryId = (await _db.QueryAsync<int>(insertDeliveryQuery, new
                {
                    _delivery.RecipientName,
                    _delivery.RecipientPhone,
                    _delivery.RecipientAddress,
                    _delivery.RecipientCityId,
                    _delivery.RecipientEmail,
                    _delivery.Weight,
                    _delivery.Volume,
                    _delivery.DeclaredPrice,
                    _delivery.PaymentMethod,
                    _delivery.CashOnDelivery,
                    _delivery.CargoInvoice,
                    OrderId = _clientOrderMapping.LegacyId
                }, transaction)).FirstOrDefault();


                var insertConnectionDocumentsQuery = @"insert into dbo.[connected_documents]
                                                     ([type1],[doc1ID],type2,doc2ID)
                                                     values (@Type1,@OrderId,@Type2,@DeliveryId)";
                await _db.ExecuteAsync(insertConnectionDocumentsQuery, new
                {
                    Type1 = 1,
                    OrderId = _clientOrderMapping.LegacyId,
                    Type2 = 16,
                    DeliveryId = newDeliveryId
                }, transaction);

                transaction.Commit();

                await _clientOrderDeliveryMapRepository.SaveAsync(new ExternalMap(
                    Guid.NewGuid(),
                    newDeliveryId,
                    _delivery.Id)
                );
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }
    }
}
