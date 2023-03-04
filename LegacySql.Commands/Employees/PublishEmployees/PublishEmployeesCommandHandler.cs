using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Employees.Export;
using MessageBus.Employees.Export.Add;
using MessageBus.Employees.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.Employees.PublishEmployees
{
    public class PublishEmployeesCommandHandler : ManagedCommandHandler<PublishEmployeesCommand>
    {
        private readonly ILegacyEmployeeRepository _legacyEmployeeRepository;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishEmployeesCommandHandler(
            ILegacyEmployeeRepository legacyEmployeeRepository,
            IEmployeeMapRepository employeeMapRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            IBus bus, ILogger<PublishEmployeesCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _legacyEmployeeRepository = legacyEmployeeRepository;
            _employeeMapRepository = employeeMapRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _bus = bus;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishEmployeesCommand command, CancellationToken cancellationToken)
        {
                await Publish(cancellationToken);
        }

        private async Task Publish(CancellationToken cancellationToken)
        {
            var (employees, lastDate) = await _legacyEmployeeRepository.GetAllAsync(cancellationToken);

            foreach (var employee in employees)
            {
                var employeeDto = MapToDto(employee);
                if (employee.IsChanged())
                {
                    //var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyEmployeeMessage, EmployeeDto>(employee.Id.ExternalId.Value, employeeDto);
                    //await _bus.Publish(message, cancellationToken);

                    //_sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                    continue;
                }

                if (employee.IsNew())
                {
                    var message = _messageFactory.CreateNewEntityMessage<AddEmployeeMessage, EmployeeDto>(employeeDto);
                    await _bus.Publish(message, cancellationToken);
                    
                    _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);

                    await _employeeMapRepository.SaveAsync(new ExternalMap(message.MessageId, employee.Id.InnerId));
                }
            }

            if (lastDate.HasValue)
            {
                await _lastChangedDateRepository.SetAsync(typeof(Employee), lastDate.Value);
            }
        }

        private EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Code = employee.Id.InnerId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Fired = employee.Fired,
                SqlLogin = employee.NickName,
                FullName = employee.FullName,
                MiddleName = employee.MiddleName,
                IndividualTaxNumber = employee.IndividualTaxNumber,
                InternalPhone = employee.InternalPhone,
                WorkPhone = employee.WorkPhone,
                Email = employee.Email,
                PassportSerialNumber = employee.PassportSerialNumber,
                PassportIssuer = employee.PassportIssuer,
                PassportSeries = employee.PassportSeries,
                PassportIssuedAt = employee.PassportIssuedAt,
            };
        }
    }
}