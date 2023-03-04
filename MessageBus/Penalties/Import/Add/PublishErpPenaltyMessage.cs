namespace MessageBus.Penalties.Import.Add
{
    public class PublishErpPenaltyMessage : BaseMessage
    {
        public ErpPenaltyDto Value { get; set; }
    }
}
