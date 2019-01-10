using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaia.Data.EFCore.Entities
{
    public class Farm : BaseEntity<Guid>
    {
        [ForeignKey(nameof(OwnerId))]
        public Farmer Owner { get; set; }
        public Guid OwnerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LocationDescription { get; set; }

        [Column(TypeName = "ntext")]
        public string Area { get; set; }

        public virtual ICollection<Cooperative> Cooperatives { get; set; } = new HashSet<Cooperative>();
        public virtual ICollection<ProductCategory> Products { get; set; } = new HashSet<ProductCategory>();
        public virtual ICollection<HarvestBatch> Harvests { get; set; } = new HashSet<HarvestBatch>();
    }

    public class ProductCategory : BaseEntity<Guid>
    {
        [ForeignKey(nameof(FarmProduceId))]
        public virtual FarmProduce FarmProduce { get; set; }
        public Guid FarmProduceId { get; set; }

        [ForeignKey(nameof(FarmId))]
        public virtual Farm Farm { get; set; }
        public Guid FarmId { get; set; }
    }
}
