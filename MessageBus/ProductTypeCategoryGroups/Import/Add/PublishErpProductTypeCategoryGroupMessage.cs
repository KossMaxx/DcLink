namespace MessageBus.ProductTypeCategoryGroups.Import.Add
{
    public class PublishErpProductTypeCategoryGroupMessage : BaseMessage
    {
        public ProductTypeCategoryGroupErpDto Value { get; set; }
    }
}
