using System;
using Axis.Pollux.Common.Models;

namespace Gaia.Core.Models
{
    public class Harvest: BaseModel<Guid>
    {
        public HarvestBatch Batch { get; set; }
        public Unit Unit { get; set; }
        public double UnitAmount { get; set; }
        public FarmProduce Produce { get; set; }
    }
}
