using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ReconciliationActs.AddReconciliationActMap
{
    public class AddReconciliationActMapCommand : BaseMapCommand
    {
        public AddReconciliationActMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId) { }
    }
}