using MessageBus.Penalties.Import;

namespace MessageBus.Bonuses.Import.Add
{
    public class PublishErpBonusMessage : BaseMessage
    {
        public ErpBonusDto Value { get; set; }
    }
}
