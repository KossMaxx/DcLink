using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_segmentation")]
    public class MarketSegmentEF
    {
        public short Id { get; set; }
        public string Title { get; set; }
    }
}
