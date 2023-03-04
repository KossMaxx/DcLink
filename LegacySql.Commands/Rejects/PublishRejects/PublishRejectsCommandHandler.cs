using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Rejects.Export;
using MessageBus.Rejects.Export.Add;
using MessageBus.Rejects.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.Rejects.PublishRejects
{
    public class PublishRejectsCommandHandler : ManagedCommandHandler<PublishRejectsCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyRejectRepository _legacyRejectRepository;
        private readonly IRejectMapRepository _rejectMapRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishRejectsCommandHandler(
            ILegacyRejectRepository legacyRejectRepository,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            IRejectMapRepository rejectMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILogger<PublishRejectsCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _legacyRejectRepository = legacyRejectRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _rejectMapRepository = rejectMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishRejectsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        public async Task Publish(PublishRejectsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.Reject);

            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            IAsyncEnumerable<Reject> rejects;
            var lastChangeDates = new List<DateTime>();

            if (command.Id.HasValue)
            {
                rejects = GetRejectAsync(command.Id.Value, cancellationToken);
            }
            else if (command.OnlyOpen)
            {
                rejects = _legacyRejectRepository.GetOpenRejectsAsync(cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Reject));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate.Value);
                }
                rejects = _legacyRejectRepository.GetChangedRejectsAsync(lastChangedDate, notFullMappingIds, cancellationToken);
            }

            await foreach (var reject in rejects)
            {
                if (reject.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(reject.ChangedAt.Value);
                }

                var mappingInfo = reject.IsMappingsFull();

                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(reject.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(reject.Id.InnerId, MappingTypes.Reject, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var rejectDto = MapToDto(reject);
                    if (reject.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyRejectMessage, RejectDto>(reject.Id.ExternalId.Value, rejectDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, reject.Id.InnerId);
                    }

                    if (reject.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddRejectMessage, RejectDto>(rejectDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, reject.Id.InnerId);

                        await _rejectMapRepository.SaveAsync(new ExternalMap(message.MessageId, reject.Id.InnerId));
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(reject.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(reject.Id.InnerId, MappingTypes.Reject));
                    }
                }
            }

            if (!command.Id.HasValue && !command.OnlyOpen && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(Reject), lastChangeDates.Max());
            }
        }

        private RejectDto MapToDto(Reject reject)
        {
            return new RejectDto
            {
                Number = reject.Id.InnerId,
                CreatedAt = reject.CreatedAt,
                Date = reject.Date,
                SerialNumber = reject.SerialNumber,
                ClientTitle = reject.ClientTitle,
                ClientId = reject.ClientId?.ExternalId,
                StatusForClient = reject.StatusForClient,
                WarehouseId = reject.WarehouseId.ExternalId.Value,
                ResponsibleForStatus = reject.ResponsibleForStatus,
                RepairType = reject.RepairType,
                DefectDescription = reject.DefectDescription,
                KitDescription = reject.KitDescription,
                ProductStatusDescription = reject.ProductStatusDescription,
                Notes = reject.Notes,
                ProductStatus = reject.ProductStatus,
                ClientOrderId = reject.ClientOrderId?.ExternalId,
                ClientOrderDate = reject.ClientOrderDate,
                ReceiptDocumentDate = reject.ReceiptDocumentDate,
                ReceiptDocumentId = reject.ReceiptDocumentId,
                SupplierId = reject.SupplierId?.ExternalId,
                SupplierTitle = reject.SupplierTitle,
                PurchasePrice = reject.PurchasePrice,
                ProductMark = reject.ProductMark,
                ProductId = reject.ProductId.ExternalId.Value,
                ProductTypeId = reject.ProductTypeId?.ExternalId,
                PurchaseCurrencyPrice = reject.PurchaseCurrencyPrice,
                OutgoingWarranty = reject.OutgoingWarranty,
                DepartureDate = reject.DepartureDate,
                Amount = reject.Amount,
                ProductRefundId = reject.ProductRefundId?.ExternalId,
                BuyDocDate = reject.BuyDocDate,
                SellDocDate = reject.SellDocDate,
                ClientOrderSqlId = reject.ClientOrderId?.InnerId,
                SupplierProductId = reject.SupplierProductId?.ExternalId,
                SupplierDescription = reject.SupplierDescription,
                SupplierProductMark = reject.SupplierProductMark,
                SupplierSerialNumber = reject.SupplierSerialNumber,
                ReturnDate = reject.ReturnDate,
                ReturnType = reject.ReturnType
            };
        }

        private async IAsyncEnumerable<Reject> GetRejectAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var reject = await _legacyRejectRepository.GetRejectAsync(id, cancellationToken);

            if (reject == null)
            {
                throw new KeyNotFoundException("Бракованная накладная не найдена");
            }

            yield return reject;
        }
    }
}