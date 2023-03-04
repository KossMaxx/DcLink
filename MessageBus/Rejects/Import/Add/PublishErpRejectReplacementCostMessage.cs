namespace MessageBus.Rejects.Import.Add
{
    public class PublishErpRejectReplacementCostMessage : BaseMessage
    {
        public ErpRejectReplacementCostDto Value { get; set; }
    }
}
