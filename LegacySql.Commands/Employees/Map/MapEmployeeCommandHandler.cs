using LegacySql.Domain.Employees;
using LegacySql.Domain.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Employees
{
    public class MapEmployeeCommandHandler : IRequestHandler<MapEmployeeCommand, Unit>
    {
        private readonly IEmployeeMapRepository _employeeMapRepository;

        public MapEmployeeCommandHandler(IEmployeeMapRepository employeeMapRepository)
        {
            _employeeMapRepository = employeeMapRepository;
        }

        public async Task<Unit> Handle(MapEmployeeCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _employeeMapRepository.GetByLegacyAsync(command.InnerId);
            if (mapping == null)
            {
                mapping = new ExternalMap(Guid.NewGuid(), command.InnerId, command.ExternalId);
                await _employeeMapRepository.SaveAsync(mapping);
            }
            else
            {
                mapping.MapToExternalId(command.ExternalId);
                await _employeeMapRepository.SaveAsync(mapping, mapping.Id);
            }
            
            return new Unit();
        }
    }
}
