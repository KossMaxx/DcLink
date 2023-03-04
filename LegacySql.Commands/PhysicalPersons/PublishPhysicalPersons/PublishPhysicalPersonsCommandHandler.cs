using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.PhysicalPersons;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.PhysicalPersons.Export;
using MessageBus.PhysicalPersons.Export.Add;
using MessageBus.PhysicalPersons.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.PhysicalPersons.PublishPhysicalPersons
{
    public class PublishPhysicalPersonsCommandHandler : ManagedCommandHandler<PublishPhysicalPersonsCommand>
    {
        private readonly ILegacyPhysicalPersonRepository _legacyPhysicalPersonRepository;
        private readonly IPhysicalPersonMapRepository _physicalPersonMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishPhysicalPersonsCommandHandler(
            ILegacyPhysicalPersonRepository legacyPhysicalPersonRepository,
            IPhysicalPersonMapRepository physicalPersonMapRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            IBus bus,
            ILogger<PublishPhysicalPersonsCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _legacyPhysicalPersonRepository = legacyPhysicalPersonRepository;
            _physicalPersonMapRepository = physicalPersonMapRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _bus = bus;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishPhysicalPersonsCommand command, CancellationToken cancellationToken)
        {
            await Publish(cancellationToken);
        }

        private async Task Publish(CancellationToken cancellationToken)
        {
            var (physicalPersons, lastDate) = await _legacyPhysicalPersonRepository.GetAllAsync(cancellationToken);

            foreach (var physicalPerson in physicalPersons)
            {
                var physicalPersonDto = MapToDto(physicalPerson);
                if (physicalPerson.IsChanged())
                {
                    //var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyPhysicalPersonMessage, PhysicalPersonDto>(physicalPerson.Id.ExternalId.Value, physicalPersonDto);
                    //await _bus.Publish(message, cancellationToken);

                    //_sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                    continue;
                }

                if (physicalPerson.IsNew())
                {
                    var message = _messageFactory.CreateNewEntityMessage<AddPhysicalPersonMessage, PhysicalPersonDto>(physicalPersonDto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);

                    await _physicalPersonMapRepository.SaveAsync(new ExternalMap(message.MessageId, physicalPerson.Id.InnerId));
                }
            }

            if (lastDate.HasValue)
            {
                await _lastChangedDateRepository.SetAsync(typeof(PhysicalPerson), lastDate.Value);
            }
        }

        private PhysicalPersonDto MapToDto(PhysicalPerson physicalPerson)
        {
            return new PhysicalPersonDto
            {
                Code = physicalPerson.Id.InnerId,
                FirstName = physicalPerson.FirstName,
                LastName = physicalPerson.LastName,
                JobPosition = physicalPerson.JobPosition,
                WorkPhone = physicalPerson.WorkPhone,
                PassportSerialNumber = physicalPerson.PassportSerialNumber,
                PassportIssuer = physicalPerson.PassportIssuer,
                PassportSeries = physicalPerson.PassportSeries,
                PassportIssuedAt = physicalPerson.PassportIssuedAt,
                Email = physicalPerson.Email,
                Fired = physicalPerson.Fired,
                SqlLogin = physicalPerson.NickName,
                FullName = physicalPerson.FullName,
                MiddleName = physicalPerson.MiddleName,
                IndividualTaxNumber = physicalPerson.IndividualTaxNumber,
                InternalPhone = physicalPerson.InternalPhone,
            };
        }
    }
}