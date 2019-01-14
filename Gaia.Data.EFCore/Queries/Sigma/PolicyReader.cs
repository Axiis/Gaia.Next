using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Sigma;
using Axis.Sigma.Policy;

namespace Gaia.Data.EFCore.Queries.Sigma
{
    public class PolicyReader: IPolicyReader
    {
        public Operation<IEnumerable<Policy>> Policies()
        {
            throw new NotImplementedException();
        }

        public Operation<IEnumerable<Policy>> PolicyForResource(params IAttribute[] resourceAttributes)
        {
            throw new NotImplementedException();
        }
    }
}
