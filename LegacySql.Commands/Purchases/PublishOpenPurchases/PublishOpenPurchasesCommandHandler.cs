using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using MassTransit;
using MediatR;
using MessageBus.Purchases.Export;
using MessageBus.Purchases.Export.Add;
using MessageBus.Purchases.Export.Change;
using PurchaseDto = MessageBus.Purchases.Export.PurchaseDto;

namespace LegacySql.Commands.Purchases.PublishOpenPurchases
{
    public class PublishOpenPurchasesCommandHandler : IRequestHandler<PublishOpenPurchasesCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyPurchaseRepository _purchaseRepository;
        private readonly IPurchaseMapRepository _purchaseMapRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;

        public PublishOpenPurchasesCommandHandler(
            IBus bus,
            ILegacyPurchaseRepository purchaseRepository,
            IPurchaseMapRepository purchaseMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory, 
            ILastChangedDateRepository lastChangedDateRepository)
        {
            _bus = bus;
            _purchaseRepository = purchaseRepository;
            _purchaseMapRepository = purchaseMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _lastChangedDateRepository = lastChangedDateRepository;
        }

        public async Task<Unit> Handle(PublishOpenPurchasesCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.Purchase)).ToList();
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            IAsyncEnumerable<Purchase> purchases = _purchaseRepository.GetOpenAsync(cancellationToken);

            var lastChangeDates = new List<DateTime>();
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
                    }

                    if (purchase.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddPurchaseMessage, PurchaseDto>(purchaseDto);
                        await _bus.Publish(message, cancellationToken);

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
                await _lastChangedDateRepository.SetAsync(typeof(Purchase), lastChangeDates.Max());
            }

            return new Unit();
        }

        private PurchaseDto MapToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id.ExternalId,
                PurchaseSqlId = purchase.PurchaseSqlId,
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
                ShippingDate = purchase.ShippingDate,
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