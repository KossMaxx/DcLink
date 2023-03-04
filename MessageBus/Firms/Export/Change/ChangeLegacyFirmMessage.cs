using System;

namespace MessageBus.Firms.Export.Change
{
    public class ChangeLegacyFirmMessage : BaseSagaMessage<FirmDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
