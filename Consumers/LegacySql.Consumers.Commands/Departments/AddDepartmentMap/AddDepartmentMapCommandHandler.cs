using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Departments;
using MediatR;

namespace LegacySql.Consumers.Commands.Departments.AddDepartmentMap
{
    public class AddDepartmentMapCommandHandler : IRequestHandler<AddDepartmentMapCommand>
    {
        private IDepartmentMapRepository _departmentMapRepository;

        public AddDepartmentMapCommandHandler(IDepartmentMapRepository departmentMapRepository)
        {
            _departmentMapRepository = departmentMapRepository;
        }

        public async Task<Unit> Handle(AddDepartmentMapCommand command, CancellationToken cancellationToken)
        {
            var employeeMap = await _departmentMapRepository.GetByMapAsync(command.MessageId);
            if (employeeMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            employeeMap.MapToExternalId(command.ExternalMapId);
            await _departmentMapRepository.SaveAsync(employeeMap, employeeMap.Id);

            return new Unit();
        }
    }
}
