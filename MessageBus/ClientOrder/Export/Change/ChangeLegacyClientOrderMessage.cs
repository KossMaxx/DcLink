using System;

namespace MessageBus.ClientOrder.Export.Change
{
    public class ChangeLegacyClientOrderMessage : BaseSagaMessage<ClientOrderDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
