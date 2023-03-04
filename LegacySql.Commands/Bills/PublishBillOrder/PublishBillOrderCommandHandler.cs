using LegacySql.Commands.Shared;
using LegacySql.Domain.Bills;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Bills.Export;
using MessageBus.Bills.Export.Add;
using MessageBus.Bills.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Bills.PublishBillOrder
{
    public class PublishBillOrderCommandHandler : ManagedCommandHandler<PublishBillOrderCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly IBillMapRepository _billMapRepository;
        private readonly ILegacyBillRepository _legacyBillRepository;

        public PublishBillOrderCommandHandler(
            ILogger<PublishBillOrderCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger,
            IBillMapRepository billMapRepository, 
            ILegacyBillRepository legacyBillRepository) : base(logger, manager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _billMapRepository = billMapRepository;
            _legacyBillRepository = legacyBillRepository;
        }

        public override async Task HandleCommand(PublishBillOrderCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.Bill);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Bill));

            IAsyncEnumerable<Bill> billOrders;
            if (command.Id.HasValue)
            {
                billOrders = GetBillOrderAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                billOrders = _legacyBillRepository.GetChangedBillOrdersAsync(
                    lastChangedDate, notFullMappingIds, cancellationToken);
            }

            var lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            await foreach (var bill in billOrders)
            {
                lastDate.Add(bill.ChangedAt);

                var mappingInfo = bill.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    if (!notFullMappingsIdsDictionary.ContainsKey(bill.Id.InnerId))
                    {
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(bill.Id.InnerId,
                            MappingTypes.Bill, DateTime.Now, mappingInfo.Why));
                    }

                    continue;
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (bill.IsChanged())
                    {
                        var billDto = MapToDto(bill);
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyBillMessage, BillDto>(bill.Id.ExternalId.Value, billDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Number, JsonConvert.SerializeObject(billDto));
                    }

                    if (bill.IsNew())
                    {
                        var billDto = MapToDto(bill);
                        var message = _messageFactory.CreateNewEntityMessage<AddBillMessage, BillDto>(billDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Number, JsonConvert.SerializeObject(billDto));

                        var mapping = new ExternalMap(message.MessageId, bill.Id.InnerId);
                        await _billMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(bill.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(bill.Id.InnerId,
                            MappingTypes.Bill));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(Bill), lastDate.Max());
            }
        }

        private BillDto MapToDto(Bill bill)
        {
            return new BillDto
            {
                SqlId = bill.Id.InnerId,
                Date = bill.Date,
                ClientId = bill.ClientId?.ExternalId,
                Comments = bill.Comments,
                SellerOkpo = bill.SellerOkpo,
                ValidToDate = bill.ValidToDate,
                FirmOkpo = bill.FirmOkpo,
                FirmSqlId = bill.FirmSqlId,
                CreatorId = bill.CreatorId?.ExternalId,
                ManagerId = bill.ManagerId?.ExternalId,
                Number = bill.Number,
                Issued = bill.Issued,
                Quantity = bill.Quantity,
                Amount = bill.Amount,
                TotalUah = bill.TotalUah,
                Total = bill.Total,
                Items = bill.Items.Select(e => new BillItemDto
                {
                    NomenclatureId = e.NomenclatureId.ExternalId.Value,
                    Quantity = e.Quantity,
                    Price = e.Price,
                    PriceUAH = e.PriceUAH,
                    Warranty = e.Warranty
                }),
                Delivery = bill.Delivery != null
                    ? new BillDeliveryDto
                    {
                        Method = bill.Delivery.Method != null
                            ? new BillDeliveryMethodDto
                            {
                                Carrier = new BillDeliveryMethodCarrierDto
                                {
                                    Id = bill.Delivery.Method.Carrier.Id,
                                    Title = bill.Delivery.Method.Carrier.Title
                                },
                                Type = new BillDeliveryMethodTypeDto
                                {
                                    Id = bill.Delivery.Method.Type.Id,
                                    Title = bill.Delivery.Method.Type.Title
                                }
                            }
                            : null,
                        Recipient = new BillDeliveryRecipientDto
                        {
                            Address = new BillDeliveryRecipientAddressDto
                            {
                                City = bill.Delivery.Recipient.Address.City,
                                Title = bill.Delivery.Recipient.Address.Title,
                                CityId = bill.Delivery.Recipient.Address.CityId,
                            },
                            Name = bill.Delivery.Recipient.Name,
                            Phone = bill.Delivery.Recipient.Phone,
                            Email = bill.Delivery.Recipient.Email,
                        },
                        Weight = bill.Delivery.Weight,
                        Volume = bill.Delivery.Volume,
                        DeclaredPrice = bill.Delivery.DeclaredPrice,
                        PayerType = bill.Delivery.PayerType,
                        PaymentMethod = bill.Delivery.PaymentMethod,
                        CargoType = bill.Delivery.CargoType,
                        ServiceType = bill.Delivery.ServiceType,
                        CashOnDelivery = bill.Delivery.CashOnDelivery,
                        Warehouse = bill.Delivery.Warehouse == null
                            ? null
                            : new BillDeliveryWarehouseDto
                            {
                                Id = bill.Delivery.Warehouse.Id,
                                Number = bill.Delivery.Warehouse.Number,
                            },
                        CargoInvoice = bill.Delivery.CargoInvoice
                    }
                    : null,
            };
        }

        private async IAsyncEnumerable<Bill> GetBillOrderAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var bill = await _legacyBillRepository.GetBillAsync(id, cancellationToken);
            if (bill == null)
            {
                throw new KeyNotFoundException("Счет-заказ не найден");
            }

            yield return bill;
        }
    }
}
