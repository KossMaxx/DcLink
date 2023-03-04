using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Departments.AddDepartmentMap
{
    public class AddDepartmentMapCommand : BaseMapCommand
    {
        public AddDepartmentMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
