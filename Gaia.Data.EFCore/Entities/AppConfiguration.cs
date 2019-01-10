using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Luna.Common;

namespace Gaia.Data.EFCore.Entities
{
    public class AppConfiguration: BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Scope { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual AppConfiguration Parent { get; set; }
        public Guid ParentId { get; set; }
        public ICollection<AppConfiguration> Children { get; set; } = new HashSet<AppConfiguration>();
    }
}