namespace MessageBus.FreeDocuments.Import.Add
{
    public class PublishErpFreeDocumentMessage : BaseMessage
    {
        public ErpFreeDocumentDto Value { get; set; }
    }
}
