using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Employees;
using MediatR;

namespace LegacySql.Consumers.Commands.Employees.AddEmployeeMap
{
    public class AddEmployeeMapCommandHandler : IRequestHandler<AddEmployeeMapCommand>
    {
        private IEmployeeMapRepository _employeeMapRepository;

        public AddEmployeeMapCommandHandler(IEmployeeMapRepository employeeMapRepository)
        {
            _employeeMapRepository = employeeMapRepository;
        }

        public async Task<Unit> Handle(AddEmployeeMapCommand command, CancellationToken cancellationToken)
        {
            var employeeMap = await _employeeMapRepository.GetByMapAsync(command.MessageId);
            if (employeeMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            employeeMap.MapToExternalId(command.ExternalMapId);
            await _employeeMapRepository.SaveAsync(employeeMap, employeeMap.Id);

            return new Unit();
        }
    }
}
