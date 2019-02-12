using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Sigma;
using Axis.Sigma.Policy;
using Microsoft.EntityFrameworkCore;
using Axis.Pollux.Common.Contracts;
using Axis.Proteus.Ioc;
using System;
using Axis.Pollux.Authorization.Abac.Models;
using Gaia.Core.Contracts;
using Gaia.Core.Exceptions;

using static Axis.Luna.Extensions.ExceptionExtension;
using static Axis.Luna.Extensions.Common;
using Axis.Jupiter.EFCore;

namespace Gaia.Data.EFCore.Queries.Sigma
{
    public class PolicyQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public PolicyQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }


        public async Task<PolluxPolicy[]> GetAllPolicies()
        {
            //pull in all policies, transform them to models
            var policies = (await _query
                .Query<Entities.Authorization.Policy>(p => p.Parent)
                .ToListAsync())
                .TransformQuery<Entities.Authorization.Policy, PolluxPolicy>(_transformer, TransformCommand.Query);

            return ConstructPolicyTrees(policies);
        }

        public async Task<PolluxPolicy[]> FilterPoliciesByResources(IAttribute[] resourceAttributes)
        {
            var resources = resourceAttributes
                .Select(_att => _att.ToString())
                .ToArray();

            //use CTE query to pull in all hierarchies of policies filtering by the resources
            var policies = (await _query.EFContext
                .Set<Entities.Authorization.Policy>()
                .FromSql(GenerateCTE(resources.Length), resources)
                .ToListAsync())
                .TransformQuery<Entities.Authorization.Policy, PolluxPolicy>(_transformer, TransformCommand.Query);

            return ConstructPolicyTrees(policies);
        }


        private static PolluxPolicy[] ConstructPolicyTrees(PolluxPolicy[] policies)
        {
            var policyKeyMap = policies
                .Select(policy => policy.Id.ValuePair(new List<PolluxPolicy>()))
                .ToDictionary();

            var rootPolicies = new List<PolluxPolicy>();

            policies.ForAll(policy =>
            {
                if (policyKeyMap.ContainsKey(policy.Parent.Id))
                    policyKeyMap[policy.Parent.Id].Add(policy);

                else rootPolicies.Add(policy);
            });

            //add all the child policies
            policies.ForAll(policy =>
            {
                var children = policyKeyMap[policy.Id];
                policy.SubPolicies = children.ToArray();
            });

            return rootPolicies.ToArray();
        }

        private static string AttributePredicates(int resourceCount)
        => Enumerable.Range(0, resourceCount)
            .Select(index => $"policy.{nameof(Entities.Authorization.Policy.GovernedResources)} LIKE {{{index}}}")
            .JoinUsing(" AND ");
        
        private static string GenerateCTE(int resourceCount)
        {
            var query =
            #region CTE Query
                $@"
WITH PolicyTrees (Id, [rank])
AS
(
-- Anchor member definition
    SELECT policy.Id, 0
    FROM Policies AS policy
	WHERE {AttributePredicates(resourceCount)}

-- Recursive member definition
    UNION ALL
    SELECT child.Id, prev.[rank] + 1
    FROM PolicyTrees as prev
    JOIN Policies AS child ON child.ParentId = prev.Id
)


-- Statement that executes the CTE
SELECT r.*, children.rank
FROM Policies AS r
JOIN PolicyTrees AS children ON children.Id = r.Id
ORDER BY children.[rank]
";
            #endregion

            return query;
        }
    }

    /// <summary>
    /// This service is the layer that should cache the policies.
    /// This service will also be the place where Policy Rules are bound to the policies using some kind of service
    /// </summary>
    public class PolicyReader: IPolicyReader
    {
        private readonly PolicyQueries _query;
        private readonly ICache _cache;
        private readonly IServiceResolver _resolver;

        public PolicyReader(PolicyQueries query, ICache cache, IServiceResolver resolver)
        {
            ThrowNullArguments(
                () => query,
                () => cache,
                () => resolver);

            _query = query;
            _cache = cache;
            _resolver = resolver;
        }

        public Operation<IEnumerable<Policy>> Policies()
        => Operation.Try(async () =>
        {
            return (await _query
                .GetAllPolicies())
                .Cast<Policy>();
        });

        public Operation<IEnumerable<Policy>> PolicyForResource(params IAttribute[] resourceAttributes)
        => Operation.Try(async () =>
        {
            if (resourceAttributes == null || resourceAttributes.Length == 0)
                return new Policy[0].AsEnumerable();

            var key = GeneratePolicyCacheKey(resourceAttributes);
            return await _cache
                .Set(key, false, null, _key => Operation.Try(async () =>
                {
                    //generate a new one since every local variable in this lambda is captured
                    var __query = _resolver.Resolve<PolicyQueries>();
                    var bindRule = CreateBinder(_resolver.Resolve<IPolicyRuleResolver>());
                    return (await __query
                        .FilterPoliciesByResources(resourceAttributes))
                        .Select(bindRule)
                        .ToArray()
                        .AsEnumerable();
                }))
                .GetOrRefresh<Policy[]>(key);
        });

        private string GeneratePolicyCacheKey(IAttribute[] resourceAttributes)
        => resourceAttributes
            .Select(_att => _att.ToString())
            .Pipe(_atts => $"Gaia.Authorization.Policies::{ValueHash(_atts)}");

        private Func<PolluxPolicy, PolluxPolicy> CreateBinder(IPolicyRuleResolver resolver)
        {
            if (resolver == null)
                throw new GaiaException(ErrorCodes.DomainLogicError);

            return policy =>
            {
                var rule = resolver
                    .ResolveRule(policy.Code)
                    .Resolve()
                    .ThrowIfNull(new GaiaException(ErrorCodes.DomainLogicError));

                policy.Rules = rule.Enumerate();
                return policy;
            };
        }
    }
}
