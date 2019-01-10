using System;
using System.Collections.Generic;
using System.Text;
using Axis.Pollux.Common.Models;

namespace Gaia.Core.Models
{
    public class FarmProduce: BaseModel<Guid>
    {
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string LocalName { get; set; }
        public string Description { get; set; }

        //other info can eventually come here
    }
}
