namespace MessageBus.PartnerProductGroups.Import.Add
{
    public class PublishErpPartnerProductGroupMessage : BaseMessage
    {
        public ErpPartnerProductGroupDto Value { get; set; }
    }
}
