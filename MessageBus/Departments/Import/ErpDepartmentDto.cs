using System;

namespace MessageBus.Departments.Import
{
    public class ErpDepartmentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid BossId { get; set; }
    }
}
