namespace MessageBus.Clients.Import.Add
{
    public class PublishErpClientMessage : BaseMessage
    {
        public ErpClientDto Value { get; set; }
    }
}
