using System;

namespace MessageBus.PartnerProductGroups.Export.Change
{
    public class ChangePartnerProductGroupMessage : BaseSagaMessage<PartnerProductGroupDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
