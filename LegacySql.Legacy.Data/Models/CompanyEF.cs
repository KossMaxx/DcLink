using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("OOO")]
    public class CompanyEF 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Okpo { get; set; }
    }
}
