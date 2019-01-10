using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class Cooperative : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }

        public virtual ICollection<CooperativeAdmin> Admins { get; set; } = new HashSet<CooperativeAdmin>();
        public virtual ICollection<Farm> Farms { get; set; } = new HashSet<Farm>();

        public Core.Models.CooperativeStatus Status { get; set; }
    }

    public class CooperativeAdmin : BaseEntity<Guid>
    {
        [ForeignKey(nameof(CooperativeId))]
        public virtual Cooperative Cooperative { get; set; }
        public Guid CooperativeId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }

    public class CooperativeFarm : BaseEntity<Guid>
    {
        [ForeignKey(nameof(CooperativeId))]
        public virtual Cooperative Cooperative { get; set; }
        public Guid CooperativeId { get; set; }

        [ForeignKey(nameof(FarmId))]
        public Farm Farm { get; set; }
        public Guid FarmId { get; set; }
    }
}
