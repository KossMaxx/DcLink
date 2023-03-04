using System;

namespace MessageBus.Manufacturer.Import.Add
{
    public class AddedManufacturerMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
