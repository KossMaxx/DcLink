using LegacySql.Domain.Employees;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Employees
{
    public class UnmapEmployeeCommandHandler : IRequestHandler<UnmapEmployeeCommand, Unit>
    {
        private readonly IEmployeeMapRepository _employeeMapRepository;

        public UnmapEmployeeCommandHandler(IEmployeeMapRepository employeeMapRepository)
        {
            _employeeMapRepository = employeeMapRepository;
        }

        public async Task<Unit> Handle(UnmapEmployeeCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _employeeMapRepository.GetByErpAsync(command.ErpId);
            await _employeeMapRepository.DeleteByIdAsync(mapping.Id);

            return new Unit();
        }
    }
}
