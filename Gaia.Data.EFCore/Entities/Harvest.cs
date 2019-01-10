using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaia.Data.EFCore.Entities
{
    public class Harvest : BaseEntity<Guid>
    {
        public Core.Models.Unit Unit { get; set; }
        public double UnitAmount { get; set; }

        [ForeignKey(nameof(HarvestBatchId))]
        public virtual HarvestBatch Batch { get; set; }
        public Guid HarvestBatchId { get; set; }

        [ForeignKey(nameof(ProduceId))]
        public virtual FarmProduce Produce { get; set; }
        public Guid ProduceId { get; set; }
    }
}
