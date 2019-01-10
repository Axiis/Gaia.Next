using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class Farmer : BaseEntity<Guid>
    {
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }

        public string EnterpriseName { get; set; }
        public Core.Models.FarmerStatus Status { get; set; }

        public virtual ICollection<Farm> Farms { get; set; } = new HashSet<Farm>();
    }
}
