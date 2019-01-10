using System;
using System.ComponentModel.DataAnnotations.Schema;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class UserIdentity : BaseEntity<Guid>
    {
        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
        
        public string UserName { get; set; }
    }
}
