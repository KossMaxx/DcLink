namespace MessageBus.MovementOrders.Import.Add
{
    public class PublishErpMovementOrderMessage : BaseMessage
    {
        public ErpMovementOrderDto Value { get; set; }
    }
}
