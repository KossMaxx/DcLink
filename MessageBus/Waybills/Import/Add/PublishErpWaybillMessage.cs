namespace MessageBus.Waybills.Import.Add
{
    public class PublishErpWaybillMessage : BaseMessage
    {
        public ErpWaybillDto Value { get; set; }
    }
}
