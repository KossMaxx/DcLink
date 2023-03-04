using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_Countries")]
    public class CountryIsoCodeEF
    {
        public int Id { get; set; }
        public string Code { get; set; }
    }
}
