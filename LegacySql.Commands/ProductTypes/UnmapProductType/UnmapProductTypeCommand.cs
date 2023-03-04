using System;
using MediatR;

namespace LegacySql.Commands.ProductTypes.UnmapProductType
{
    public class UnmapProductTypeCommand : IRequest
    {
        public UnmapProductTypeCommand(Guid erpId)
        {
            ErpId = erpId;
        }

        public Guid ErpId { get; }
    }
}
