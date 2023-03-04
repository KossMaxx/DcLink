using System;
using MediatR;

namespace LegacySql.Commands.Cashboxes.RemoveCashboxHandMapping
{
    public class RemoveCashboxHandMappingCommand : IRequest
    {
        public RemoveCashboxHandMappingCommand(Guid? erpId, int? id = null)
        {
            Id = id;
            ErpId = erpId;
        }

        public int? Id { get; }
        public Guid? ErpId { get; }
    }
}
