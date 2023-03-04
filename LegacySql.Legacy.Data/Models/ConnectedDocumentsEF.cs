using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("connected_documents")]
    public class ConnectedDocumentsEF
    {
        public int Id { get; set; }
        public byte Type1 { get; set; }
        public int Doc1Id { get; set; }
        public int Doc2Id { get; set; }
        public byte Type2 { get; set; }
        public DateTime? Date { get; set; }
    }
}
