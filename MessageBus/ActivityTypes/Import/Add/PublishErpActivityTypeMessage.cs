namespace MessageBus.ActivityTypes.Import.Add
{
    public class PublishErpActivityTypeMessage : BaseMessage
    {
        public ErpActivityTypeDto Value { get; set; }
    }
}
