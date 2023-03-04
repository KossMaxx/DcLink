using System;

namespace MessageBus
{
    public class BaseSagaMessage<TMessageValue>
    {
        public Guid SagaId { get; set; }
        public Guid MessageId { get; set; }
        public TMessageValue Value { get; set; }
    }
}
