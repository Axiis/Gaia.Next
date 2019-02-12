using Axis.Sigma.Policy;
using System;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Core.Models
{
    public class PolicyRule: Rule
    {
        public PolicyRule(IGaiaPolicyExpression policyExpression)
        {
            ThrowNullArguments(() => policyExpression);

            this.Expression = policyExpression;
            this.EvaluationEffect = policyExpression.RuleEvaluationEffect;
            this.Id = policyExpression.RuleId;
            this.PolicyCode = policyExpression.PolicyCode;
            this.Code = $"{policyExpression.PolicyCode}::{Id}";
        }

        public string PolicyCode { get; }
    }

    public interface IGaiaPolicyExpression: IPolicyExpression
    {
        /// <summary>
        /// A string identifying the Code of the policy to which this rule applies
        /// </summary>
        string PolicyCode { get; }
        Guid RuleId { get; }
        Effect RuleEvaluationEffect { get; }
    }
}
