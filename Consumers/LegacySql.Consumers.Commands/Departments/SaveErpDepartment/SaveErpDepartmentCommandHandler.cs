using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Departments;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Departments.Import;
using Newtonsoft.Json;

namespace LegacySql.Consumers.Commands.Departments.SaveErpDepartment
{
    public class SaveErpDepartmentCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpDepartmentDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private ErpDepartmentSaver _erpDepartmentSaver;
        private readonly IDepartmentMapRepository _departmentMapRepository;

        public SaveErpDepartmentCommandHandler(IErpNotFullMappedRepository erpNotFullMappedRepository, 
            ErpDepartmentSaver erpDepartmentSaver, 
            IDepartmentMapRepository departmentMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpDepartmentSaver = erpDepartmentSaver;
            _departmentMapRepository = departmentMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpDepartmentDto> command, CancellationToken cancellationToken)
        {
            var department = command.Value;
            var departmentMapping = await _departmentMapRepository.GetByErpAsync(department.Id);
            _erpDepartmentSaver.InitErpObject(department, departmentMapping);

            var mappingInfo = await _erpDepartmentSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(department, mappingInfo.Why);
                return new Unit();
            }

            await _erpDepartmentSaver.Save(command.MessageId);
            await _erpNotFullMappedRepository.RemoveAsync(department.Id, MappingTypes.Department);
            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpDepartmentDto department, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                department.Id,
                MappingTypes.Department,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(department)
            ));
        }
    }
}
