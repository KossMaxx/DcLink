namespace MessageBus.Employees.Import.Add
{
    public class PublishErpEmployeeMessage : BaseMessage
    {
        public ErpEmployeeDto Value { get; set; }
    }
}
