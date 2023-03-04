using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_CountryNames")]
    public class CountryEF
    {
        public int Id { get; set; }
        public int IsoId { get; set; }
        public virtual CountryIsoCodeEF Iso { get; set; }
        public string Title { get; set; }
        public int LanguageId { get; set; }
        public virtual LanguageEF Language { get; set; }
    }
}
