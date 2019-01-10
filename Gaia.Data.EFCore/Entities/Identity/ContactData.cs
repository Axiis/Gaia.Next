using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class ContactData : BaseEntity<Guid>
    {
        public string Data { get; set; }
        public string Channel { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// comma separated list of values (with commas' encoded away)
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Tags { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }
}