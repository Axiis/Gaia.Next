using Axis.Luna.Operation;
using Axis.Sigma.Policy;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface IPolicyRuleResolver
    {
        Operation<PolicyRule> ResolveRule(string policyCode);
    }
}
