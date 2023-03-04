namespace MessageBus.Departments.Import.Add
{
    public class PublishErpDepartmentMessage : BaseMessage
    {
        public ErpDepartmentDto Value { get; set; }
    }
}
