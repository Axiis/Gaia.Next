using System;
using Axis.Pollux.Common.Models;
using Axis.Pollux.Identity.Models;

namespace Gaia.Core.Models
{
    public class Farmer: BaseModel<Guid>
    {
        public string EnterpriseName { get; set; }
        public User User { get; set; }
        public Farm[] Farms { get; set; }
        public FarmerStatus Status { get; set; }
    }

    public enum FarmerStatus
    {
        Active,
        Disabled
    }
}
