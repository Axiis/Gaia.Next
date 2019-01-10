using System;
using Axis.Pollux.Common.Models;

namespace Gaia.Core.Models
{
    public class HarvestBatch: BaseModel<Guid>
    {
        public Farm Farm { get; set; }
        public string BatchTitle { get; set; }
        public DateTimeOffset BatchDate { get; set; }
        public Harvest[] Harvests { get; set; }

        public HarvestBatchStatus Status { get; set; }
    }

    public enum HarvestBatchStatus
    {
        Draft = 0,
        Published
    }
}
