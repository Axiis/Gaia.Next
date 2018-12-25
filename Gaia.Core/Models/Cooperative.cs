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
        public User[] Admins { get; set; }
        public Farm[] Farms { get; set; }

        public CooperativeStatus Status { get; set; }
    }

    public enum CooperativeStatus
    {
        Active,
        Disabled
    }
}
