namespace MessageBus.SegmentationTurnovers.Import.Add
{
    public class PublishErpSegmentationTurnoverMessage : BaseMessage
    {
        public ErpSegmentationTurnoverDto Value { get; set; }
    }
}
