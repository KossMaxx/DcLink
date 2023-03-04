using MessageBus;
using System;

namespace LegacySql.Commands
{
    public interface ISqlMessageFactory
    {
        TMessage CreateNewEntityMessage<TMessage, TMessageValue>(TMessageValue value) where TMessage : BaseSagaMessage<TMessageValue>, new();
        TMessage CreateChangedEntityMessage<TMessage, TMessageValue>(Guid erpId, TMessageValue value) where TMessage : BaseSagaMessage<TMessageValue>, IMappedMessage, new();
    }
}
