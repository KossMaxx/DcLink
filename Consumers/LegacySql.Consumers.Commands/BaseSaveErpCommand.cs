using System;
using MediatR;

namespace LegacySql.Consumers.Commands
{
    public class BaseSaveErpCommand<T> : IRequest where T: class
    {
        public BaseSaveErpCommand(T value, Guid messageId)
        {
            Value = value;
            MessageId = messageId;
        }

        public T Value { get; }
        public Guid MessageId { get; }
    }
}
