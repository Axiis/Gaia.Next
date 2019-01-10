using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gaia.Data.EFCore.Entities
{
    public class BaseEntity<TKey>
    {
        protected BaseEntity()
        { }

        [Key]
        public TKey Id { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
