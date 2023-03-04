using System.Collections.Generic;

namespace MessageBus.Clients.Export.Add
{
    public class AddClientMessage : BaseSagaMessage<ClientDto>
    {
        public IEnumerable<AddClientMessage> NestedMessages { get; set; }
    }
}
