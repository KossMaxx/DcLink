using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LegacySql.Domain.Shared;

namespace LegacySql.Data.Models
{
    public class BaseMapModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MapGuid { get; set; }
        public Guid? ErpGuid { get; set; }
        public int LegacyId { get; set; }
        public DateTime CreateDate { get; set; }

        public MappingStatuses MappingStatus(int entityLegacyId)
        {
            if (LegacyId == entityLegacyId && ErpGuid.HasValue)
            {
                return MappingStatuses.Permanent;
            }
            if (LegacyId == entityLegacyId && !ErpGuid.HasValue)
            {
                return MappingStatuses.Temporary;
            }
            if (LegacyId != entityLegacyId && ErpGuid.HasValue)
            {
                return MappingStatuses.Wrong;
            }

            throw new ApplicationException($"Something wrong with mapping Id:{Id}");
        }
    }
}
