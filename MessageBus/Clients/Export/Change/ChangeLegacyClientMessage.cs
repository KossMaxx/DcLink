using System;
using System.Collections.Generic;

namespace MessageBus.Clients.Export.Change
{
    public class ChangeLegacyClientMessage : BaseSagaMessage<ClientDto>
    {
        public Guid? ErpId { get; set; }
        public IEnumerable<ChangeLegacyClientMessage> NestedMessages { get; set; }
    }
}
