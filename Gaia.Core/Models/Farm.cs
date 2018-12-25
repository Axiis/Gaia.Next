using System;
using Axis.Pollux.Common.Models;
using Gaia.Core.Utils;

namespace Gaia.Core.Models
{
    public class Farm: BaseModel<Guid>
    {
        public Farmer Owner { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LocationDescription { get; set; }
        public GeoPosition[] Area { get; set; }
        public Cooperative[] Cooperatives { get; set; }

        public FarmProduct[] Products { get; set; }
        public HarvestBatch[] Harvests { get; set; }
    }
}
