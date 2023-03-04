using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Purchases.Export;
using MessageBus.Purchases.Export.Add;
using MessageBus.Purchases.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using PurchaseDto = MessageBus.Purchases.Export.PurchaseDto;

namespace LegacySql.Commands.Purchases.PublishPurchases
{
    public class PublishPurchasesCommandHandler : ManagedCommandHandler<PublishPurchasesCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyPurchaseRepository _purchaseRepository;
        private readonly IPurchaseMapRepository _purchaseMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishPurchasesCommandHandler(
            IBus bus,
            ILegacyPurchaseRepository purchaseRepository,
            IPurchaseMapRepository purchaseMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            ILogger<PublishPurchasesCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _bus = bus;
            _purchaseRepository = purchaseRepository;
            _purchaseMapRepository = purchaseMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishPurchasesCommand command, CancellationToken cancellationToken)
        {
                await Publish(cancellationToken, command.Id);
        }

        public async Task Publish(CancellationToken cancellationToken, int? id)
        {
            var lastChangeDates = new List<DateTime?>();

            var notFullMappingIds = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.Purchase)).ToList();
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Purchase));
            if (lastChangedDate.HasValue)
            {
                lastChangeDates.Add(lastChangedDate);
            }

            IAsyncEnumerable<Purchase> purchases;
            if (id.HasValue)
            {
                purchases = _purchaseRepository.GetPurchaseAsync(id.Value, cancellationToken);
            }
            else
            {
                purchases = _purchaseRepository.GetChangedAsync(lastChangedDate, notFullMappingIds, cancellationToken);
            }

            await foreach (var purchase in purchases.WithCancellation(cancellationToken))
            {
                if (purchase.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(purchase.ChangedAt.Value);
                }

                var mappingInfo = purchase.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(purchase.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(purchase.Id.InnerId,
                        MappingTypes.Purchase, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var purchaseDto = MapToDto(purchase);
                    if (purchase.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyPurchaseMessage, PurchaseDto>(purchase.Id.ExternalId.Value, purchaseDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, purchase.Id.InnerId);
                    }

                    if (purchase.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddPurchaseMessage, PurchaseDto>(purchaseDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, purchase.Id.InnerId);

                        var mapping = new ExternalMap(message.MessageId, purchase.Id.InnerId);
                        await _purchaseMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(purchase.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(purchase.Id.InnerId,
                            MappingTypes.Purchase));
                    }
                }
            }

            if (lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(Purchase), lastChangeDates.Max().Value);
            }
        }

        private PurchaseDto MapToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id.ExternalId,
                PurchaseSqlId = purchase.Id.InnerId,
                Date = purchase.Date,
                SupplierId = purchase.ClientId.ExternalId,
                WarehouseId = purchase.WarehouseId?.ExternalId,
                Comments = purchase.Comments,
                SupplierDocument = purchase.SupplierDocument,
                IsExecuted = purchase.IsExecuted,
                IsPaid = purchase.IsPaid,
                EmployeeUsername = purchase.EmployeeUsername,
                PaymentDate = purchase.PaymentDate,
                RecipientOKPO = purchase.RecipientOKPO,
                FirmSqlId = purchase.FirmSqlId,
                ShippingDate = purchase.ShippingDate,
                IsActual = purchase.IsActual,
                Items = purchase.Items.Select(i => new PurchaseItemDto
                {
                    NomenclatureId = i.ProductId.ExternalId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                }),
                BillNumbers = purchase.BillNumbers
            };
        }
    }
}