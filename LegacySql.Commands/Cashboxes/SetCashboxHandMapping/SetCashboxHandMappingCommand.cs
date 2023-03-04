using System;
using MediatR;

namespace LegacySql.Commands.Cashboxes.SetCashboxHandMapping
{
    public class SetCashboxHandMappingCommand : IRequest
    {
        public SetCashboxHandMappingCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
