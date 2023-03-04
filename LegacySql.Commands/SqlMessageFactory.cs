using MessageBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Commands
{
    public class SqlMessageFactory : ISqlMessageFactory
    {
        public TMessage CreateNewEntityMessage<TMessage, TMessageValue>(TMessageValue value) where TMessage : BaseSagaMessage<TMessageValue>, new()
        {
            var message = new TMessage();

            message.SagaId = Guid.NewGuid();
            message.MessageId = Guid.NewGuid();
            message.Value = value;

            return message;
        }

        public TMessage CreateChangedEntityMessage<TMessage, TMessageValue>(Guid erpId, TMessageValue value) where TMessage : BaseSagaMessage<TMessageValue>, IMappedMessage, new()
        {
            var message = new TMessage();

            message.SagaId = Guid.NewGuid();
            message.MessageId = Guid.NewGuid();
            message.Value = value;
            message.ErpId = erpId;

            return message;
        }
    }
}
