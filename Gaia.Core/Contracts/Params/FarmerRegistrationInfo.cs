using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Models;

namespace Gaia.Core.Contracts.Params
{
    public class FarmerRegistrationInfo: IValidatable
    {
        public string AccountEmail { get; set; }
        public string AccountPassword { get; set; }
        public string EnterpriseName { get; set; }


        public Operation Validate()
        {
            throw new NotImplementedException();
        }
    }
}
