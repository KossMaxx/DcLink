using LegacySql.Commands.Shared;
using LegacySql.Domain.Rates;
using MassTransit;
using MessageBus.Rates.Export;
using MessageBus.Rates.Export.Add;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Rates.PublishRates
{
    internal class PublishRateCommandHandler : ManagedCommandHandler<PublishRateCommand>
    {
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILegacyRateRepository _legacyRateRepository;
        public PublishRateCommandHandler(
            ILogger<PublishRateCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISqlMessageFactory messageFactory, 
            ILegacyRateRepository legacyRateRepository) : base(logger, manager)
        {
            _bus = bus;
            _messageFactory = messageFactory;
            _legacyRateRepository = legacyRateRepository;
        }

        public override async Task HandleCommand(PublishRateCommand command, CancellationToken cancellationToken)
        {
            var rates = _legacyRateRepository.GetRatesAsync(cancellationToken);

            await foreach(var rate in rates)
            {
                var rateDto = MapToDto(rate);
                var message = _messageFactory.CreateNewEntityMessage<AddRateMessage, RateDto>(rateDto);
                await _bus.Publish(message, cancellationToken);
            }
        }

        private RateDto MapToDto(Rate rate)
        {
            return new RateDto
            {
                Id = rate.Id,
                Title = rate.Title,
                Value = rate.Value
            };
        }
    }
}
