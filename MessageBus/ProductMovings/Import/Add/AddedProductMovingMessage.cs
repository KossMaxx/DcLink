using System;

namespace MessageBus.ProductMovings.Import.Add
{
    public class AddedProductMovingMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
