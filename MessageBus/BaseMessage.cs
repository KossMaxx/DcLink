using System;

namespace MessageBus
{
    public class BaseMessage
    {        
        public Guid MessageId { get; set; }
        public bool IsTest { get; set; }       
    }
}
