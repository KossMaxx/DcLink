using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("departments")]
    public class DepartmentEF
    {
        public short Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BossPosition { get; set; }
        public int BossId { get; set; }
        public virtual EmployeeEF Boss { get; set; }
    }
}
