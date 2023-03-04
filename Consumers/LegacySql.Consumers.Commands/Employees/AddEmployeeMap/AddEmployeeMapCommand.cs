using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Employees.AddEmployeeMap
{
    public class AddEmployeeMapCommand : BaseMapCommand
    {
        public AddEmployeeMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        { }
    }
}
