using System;
using MediatR;

namespace LegacySql.Commands.Shared
{
    public class BaseMapCommand : IRequest
    {
        public Guid MessageId { get; set; }
        public Guid ExternalMapId { get; set; }

        public BaseMapCommand(Guid messageId, Guid externalMapId)
        {
            MessageId = messageId;
            ExternalMapId = externalMapId;
        }
    }
}
