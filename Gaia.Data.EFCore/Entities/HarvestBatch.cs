using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaia.Data.EFCore.Entities
{
    public class HarvestBatch : BaseEntity<Guid>
    {
        public string BatchTitle { get; set; }
        public DateTimeOffset BatchDate { get; set; }
        public Core.Models.HarvestBatchStatus Status { get; set; }

        [ForeignKey(nameof(FarmId))]
        public virtual Farm Farm { get; set; }
        public Guid FarmId { get; set; }

        public virtual ICollection<Harvest> Harvests { get; set; } = new HashSet<Harvest>();
    }
}