using MediatR;
using System;

namespace LegacySql.Commands.Employees
{
    public class UnmapEmployeeCommand: IRequest
    {
        public UnmapEmployeeCommand(Guid erpId)
        {
            ErpId = erpId;
        }

        public Guid ErpId { get; }
    }
}
