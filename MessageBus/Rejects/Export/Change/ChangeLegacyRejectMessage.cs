using System;

namespace MessageBus.Rejects.Export.Change
{
    public class ChangeLegacyRejectMessage : BaseSagaMessage<RejectDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
