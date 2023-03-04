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

namespace LegacySql.Commands.CashboxPayments.CashboxApplicationPaymentPublish
{
    public class PublishCashboxApplicationPaymentCommandHandler : ManagedCommandHandler<PublishCashboxApplicationPaymentCommand>
    {
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ICashboxApplicationPaymentMapRepository _cashboxApplicationPaymentMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyCashboxApplicationPaymentRepository _legacyCashboxApplicationPaymentRepository;
        private readonly ISqlMessageFactory _messageFactory;
        public PublishCashboxApplicationPaymentCommandHandler(
            ILogger<PublishCashboxApplicationPaymentCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISagaLogger sagaLogger,
            INotFullMappedRepository notFullMappedRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            ISqlMessageFactory messageFactory,
            ICashboxApplicationPaymentMapRepository cashboxApplicationPaymentMapRepository,
            ILegacyCashboxApplicationPaymentRepository legacyCashboxApplicationPaymentRepository) : base(logger, manager)
        {
            _bus = bus;
            _sagaLogger = sagaLogger;
            _notFullMappedRepository = notFullMappedRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _messageFactory = messageFactory;
            _cashboxApplicationPaymentMapRepository = cashboxApplicationPaymentMapRepository;
            _legacyCashboxApplicationPaymentRepository = legacyCashboxApplicationPaymentRepository;
        }

        public async override Task HandleCommand(PublishCashboxApplicationPaymentCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.CashboxApplicationPayment)).ToList();
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);

            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(CashboxApplicationPayment));
            List<DateTime> lastDate = lastChangedDate.HasValue
               ? new List<DateTime> { lastChangedDate.Value }
               : new List<DateTime>(); ;

            IAsyncEnumerable<CashboxApplicationPayment> payments;
            if (command.Id.HasValue)
            {
                payments = GetApplicationPaymentAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                payments = _legacyCashboxApplicationPaymentRepository.GetChangedAsync(
                    lastChangedDate,
                    notFullMappingIds,
                    cancellationToken);
            }

            await foreach (var payment in payments)
            {
                if (payment.ChangeDate.HasValue)
                {
                    lastDate.Add(payment.ChangeDate.Value);
                }
                else
                {
                    lastDate.Add(payment.Date);
                }

                var mappingInfo = payment.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(payment.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(payment.Id.InnerId,
                        MappingTypes.CashboxApplicationPayment, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var paymentDto = MapToDto(payment);
                    if (payment.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyCashboxApplicationPaymentMessage, CashboxApplicationPaymentDto>(payment.Id.ExternalId.Value, paymentDto);
                        await _bus.Publish(message, cancellationToken);
                    }

                    if (payment.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddCashboxApplicationPaymentMessage, CashboxApplicationPaymentDto>(paymentDto);
                        await _bus.Publish(message, cancellationToken);

                        var mapping = new ExternalMap(message.MessageId, payment.Id.InnerId);
                        await _cashboxApplicationPaymentMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(payment.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(payment.Id.InnerId,
                            MappingTypes.CashboxApplicationPayment));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(CashboxApplicationPayment), lastDate.Max());
            }
        }

        private CashboxApplicationPaymentDto MapToDto(CashboxApplicationPayment payment)
        {
            return new CashboxApplicationPaymentDto
            {
                SqlId = payment.Id.InnerId,
                Date = payment.Date,
                WriteOffCliectId = payment.WriteOffCliectId.ExternalId.Value,
                ReceiveClientId = payment.ReceiveClientId.ExternalId.Value,
                CurrencyId = payment.CurrencyId,
                Amount = payment.Amount,
                Description = payment.Description
            };
        }

        private async IAsyncEnumerable<CashboxApplicationPayment> GetApplicationPaymentAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var payment = await _legacyCashboxApplicationPaymentRepository.Get(id, cancellationToken);
            if (payment == null)
            {
                throw new KeyNotFoundException("Заявка на проведение кассового ордера не найдена");
            }

            yield return payment;
        }
    }
}
