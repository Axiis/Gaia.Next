using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaia.Data.EFCore.Entities
{
    public class FarmProduce : BaseEntity<Guid>
    {
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string LocalName { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        //other info can eventually come here
    }
}
