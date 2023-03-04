namespace MessageBus.Clients.Import.Add
{
    public class PublishErpPartnerMessage : BaseMessage
    {
        public ErpPartnerDto Value { get; set; }
    }
}
