using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_Languages")]
    public class LanguageEF
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
