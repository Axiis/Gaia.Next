using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class NameData : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public int Status { get; set; }


        /// <summary>
        /// comma separated list of values (with commas' encoded away)
        /// </summary>
        public string Tags { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }
}
