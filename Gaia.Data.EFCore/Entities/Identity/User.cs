using System;
using System.Collections.Generic;
using System.Text;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class User: BaseEntity<Guid>
    {
        public int Status { get; set; }
    }
}
