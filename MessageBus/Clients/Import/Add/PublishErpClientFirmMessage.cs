namespace MessageBus.Clients.Import.Add
{
    public class PublishErpClientFirmMessage : BaseMessage
    {
        public ErpFirmDto Value { get; set; }
    }
}
