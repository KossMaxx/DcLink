using LegacySql.Commands.Shared;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Cashboxes.Export;
using MessageBus.Cashboxes.Export.Add;
using MessageBus.Cashboxes.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.CashboxPayments.CashboxPaymentPublish
{
    public class PublishCashboxPaymentCommandHandler : ManagedCommandHandler<PublishCashboxPaymentCommand>
    {
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ICashboxPaymentMapRepository _cashboxPaymentMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyCashboxPaymentRepository _legacyCashboxPaymentRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishCashboxPaymentCommandHandler(
            ILogger<PublishCashboxPaymentCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISagaLogger sagaLogger,
            INotFullMappedRepository notFullMappedRepository,
            ICashboxPaymentMapRepository cashboxPaymentMapRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            ILegacyCashboxPaymentRepository legacyCashboxPaymentRepository, 
            ISqlMessageFactory messageFactory) : base(logger, manager)
        {
            _bus = bus;
            _sagaLogger = sagaLogger;
            _notFullMappedRepository = notFullMappedRepository;
            _cashboxPaymentMapRepository = cashboxPaymentMapRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _legacyCashboxPaymentRepository = legacyCashboxPaymentRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishCashboxPaymentCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.CashboxPayment)).ToList();
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);

            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(CashboxPayment));
            List<DateTime> lastDate = lastChangedDate.HasValue
               ? new List<DateTime> { lastChangedDate.Value }
               : new List<DateTime>(); ;

            IAsyncEnumerable<CashboxPayment> payments;
            if (command.Id.HasValue)
            {
                payments = GetClientOrderAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                payments = _legacyCashboxPaymentRepository.GetChangedAsync(
                    lastChangedDate,
                    notFullMappingIds,
                    cancellationToken);
            }

            await foreach(var payment in payments)
            {
                if (payment.ChangedAt.HasValue)
                {
                    lastDate.Add(payment.ChangedAt.Value);
                }

                var mappingInfo = payment.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(payment.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(payment.Id.InnerId,
                        MappingTypes.CashboxPayment, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var paymentDto = MapToDto(payment);
                    if (payment.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyCashboxPaymentMessage, CashboxPaymentDto>(payment.Id.ExternalId.Value, paymentDto);
                        await _bus.Publish(message, cancellationToken);
                    }

                    if (payment.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddCashboxPaymentMessage, CashboxPaymentDto>(paymentDto);
                        await _bus.Publish(message, cancellationToken);

                        var mapping = new ExternalMap(message.MessageId, payment.Id.InnerId);
                        await _cashboxPaymentMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(payment.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(payment.Id.InnerId,
                            MappingTypes.CashboxPayment));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(CashboxPayment), lastDate.Max());
            }
        }

        private CashboxPaymentDto MapToDto(CashboxPayment payment)
        {
            return new CashboxPaymentDto
            {
                Number = payment.Id.InnerId,
                Date = payment.Date,
                CashboxId = payment.CashboxId.ExternalId.Value,
                ClientId = payment.ClientId.ExternalId.Value,
                AmountUSD = payment.AmountUSD,
                AmountUAH = payment.AmountUAH,
                AmountEuro = payment.AmountEuro,
                Rate = payment.Rate,
                RateEuro = payment.RateEuro,
                Description = payment.Description,
                Total = payment.Total
            };
        }

        private async IAsyncEnumerable<CashboxPayment> GetClientOrderAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var payment = await _legacyCashboxPaymentRepository.Get(id, cancellationToken);
            if (payment == null)
            {
                throw new KeyNotFoundException("Кассовый ордер не найден");
            }

            yield return payment;
        }
    }
}
