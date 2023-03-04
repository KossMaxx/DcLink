using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Departments;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpDepartmentEventHandler : INotificationHandler<ResaveErpDepartmentEvent>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpDepartmentSaver _erpDepartmentSaver;
        private readonly IDepartmentMapRepository _departmentMapRepository;

        public ResaveErpDepartmentEventHandler(IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpDepartmentSaver erpDepartmentSaver, 
            IDepartmentMapRepository departmentMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpDepartmentSaver = erpDepartmentSaver;
            _departmentMapRepository = departmentMapRepository;
        }

        public async Task Handle(ResaveErpDepartmentEvent notification, CancellationToken cancellationToken)
        {
            foreach (var department in notification.Messages)
            {
                var departmentMapping = await _departmentMapRepository.GetByErpAsync(department.Id);
                _erpDepartmentSaver.InitErpObject(department, departmentMapping);

                var mappingInfo = await _erpDepartmentSaver.GetMappingInfo();
                if (!mappingInfo.IsMappingFull)
                {
                    continue;
                }

                await _erpDepartmentSaver.Save(Guid.NewGuid());
                await _erpNotFullMappedRepository.RemoveAsync(department.Id, MappingTypes.Department);
            }
        }
    }
}
