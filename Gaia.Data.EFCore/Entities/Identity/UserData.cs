using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Luna.Common;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class UserData: BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }
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
