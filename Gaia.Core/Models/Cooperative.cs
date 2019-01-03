using System;
using Axis.Pollux.Common.Models;
using Axis.Pollux.Identity.Models;

namespace Gaia.Core.Models
{
    public class Cooperative: BaseModel<Guid>
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }
        public CooperativeAdmin[] Admins { get; set; }
        public Farm[] Farms { get; set; }

        public CooperativeStatus Status { get; set; }
    }

    public class CooperativeAdmin : BaseModel<Guid>
    {
        public Cooperative Cooperative { get; set; }
        public User User { get; set; }
    }

    public class CooperativeFarm : BaseModel<Guid>
    {
        public Cooperative Cooperative { get; set; }
        public Farm Farm { get; set; }
    }

    public enum CooperativeStatus
    {
        Active,
        Disabled
    }
}
