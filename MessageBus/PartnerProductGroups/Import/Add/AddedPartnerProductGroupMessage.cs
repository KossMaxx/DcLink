using System;

namespace MessageBus.PartnerProductGroups.Import.Add
{
    public class AddedPartnerProductGroupMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
