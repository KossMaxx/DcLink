namespace MessageBus.Classes.Import.Add
{
    public class PublishErpClassMessage : BaseMessage
    {
        public ErpClassDto Value { get; set; }
    }
}
