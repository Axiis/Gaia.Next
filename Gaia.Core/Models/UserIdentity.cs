using System;
using Axis.Pollux.Common.Models;
using Axis.Pollux.Identity.Models;

namespace Gaia.Core.Models
{
    public class UserIdentity: BaseModel<Guid>
    {
        public User Owner { get; set; }
        public string UserName { get; set; }
    }
}
