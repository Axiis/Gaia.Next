using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class BioData : BaseEntity<Guid>
    {
        public DateTimeOffset? DateOfBirth { get; set; }
        public string Gender { get; set; }

        public string CountryOfBirth { get; set; }


        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }
}
