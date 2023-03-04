namespace MessageBus.Rejects.Import.Add
{
    public class PublishErpRejectMessage : BaseMessage
    {
        public ErpRejectDto Value { get; set; }
    }
}
