using System;

namespace MessageBus.Departments.Export
{
    public class DepartmentDto
    {
        public int SqlId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BossPosition { get; set; }
        public Guid BossId { get; set; }
    }
}
