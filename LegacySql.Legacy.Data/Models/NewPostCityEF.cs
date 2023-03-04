using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("NP_Cities")]
    public class NewPostCityEF
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Guid CityRef { get; set; }
    }
}
