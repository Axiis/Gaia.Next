using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Gaia.Core.Contracts;
using Gaia.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gaia.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PolicyRuleResolver : IPolicyRuleResolver
    {
        private readonly Dictionary<string, PolicyRule> _policyRules;

        public PolicyRuleResolver(IEnumerable<IGaiaPolicyExpression> policyExpressions)
        {
            _policyRules = policyExpressions
                .Select(ToPolicyRule)
                .ToDictionary(rule => rule.PolicyCode);
        }

        public Operation<PolicyRule> ResolveRule(string policyCode)
        => Operation.Try(() =>
        {
            return _policyRules.TryGetValue(policyCode, out var rule)
                ? rule
                : null;
        });

        public static PolicyRule ToPolicyRule(IGaiaPolicyExpression expression) => new PolicyRule(expression);
    }
}
