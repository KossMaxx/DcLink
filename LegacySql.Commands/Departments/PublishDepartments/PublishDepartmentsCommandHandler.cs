using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.Departments;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Departments.Export;
using MessageBus.Departments.Export.Add;
using MessageBus.Departments.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.Departments.PublishDepartments
{
    public class PublishDepartmentsCommandHandler : ManagedCommandHandler<PublishDepartmentsCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyDepartmentRepository _legacyDepartmentRepository;
        private readonly IDepartmentMapRepository _departmentMapRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishDepartmentsCommandHandler(ILogger<PublishDepartmentsCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILegacyDepartmentRepository legacyDepartmentRepository,
            IDepartmentMapRepository departmentMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, manager)
        {
            _bus = bus;
            _legacyDepartmentRepository = legacyDepartmentRepository;
            _departmentMapRepository = departmentMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishDepartmentsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappings = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.Department)).ToDictionary(m => m);
            var departments = await _legacyDepartmentRepository.GetAllAsync(notFullMappings.Select(m => m.Key).ToList(), cancellationToken);

            foreach (var department in departments)
            {
                var mappingInfo = department.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(department.Id.InnerId, MappingTypes.Department, DateTime.Now, mappingInfo.Why));
                    continue;
                }

                var departmentDto = MapToDto(department);
                if (department.IsChanged())
                {
                    var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyDepartmentMessage, DepartmentDto>(department.Id.ExternalId.Value, departmentDto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.SqlId);
                }

                if (department.IsNew())
                {
                    var message = _messageFactory.CreateNewEntityMessage<AddDepartmentMessage, DepartmentDto>(departmentDto);
                    await _bus.Publish(message);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.SqlId);

                    await _departmentMapRepository.SaveAsync(new ExternalMap(message.MessageId, department.Id.InnerId));
                }

                if (notFullMappings.ContainsKey(department.Id.InnerId))
                {
                    await _notFullMappedRepository.RemoveAsync(new NotFullMapped(department.Id.InnerId, MappingTypes.Department));
                }
            }
        }

        private DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                SqlId = department.Id.InnerId,
                Title = department.Title,
                Description = department.Description,
                BossPosition = department.BossPosition,
                BossId = department.BossId.ExternalId.Value
            };
        }
    }
}
