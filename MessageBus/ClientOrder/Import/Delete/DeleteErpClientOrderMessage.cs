namespace MessageBus.ClientOrder.Import.Delete
{
    public class DeleteErpClientOrderMessage : BaseMessage
    {
        public ErpClientOrderDeleteIdentifierDto Value { get; set; }
    }
}
