using static Axis.Luna.Extensions.ExceptionExtension;

using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Axis.Pollux.Authorization.Abac.Models;
using Gaia.Services.Queries;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Queries.Sigma
{
    public class PolicyWriterQuery : IPolicyWriterQuery
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public PolicyWriterQuery(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<PolluxPolicy> GetPolicyById(Guid policyId)
        {
            var policyEntity = await _query
                .Query<Entities.Authorization.Policy>(
                    p => p.Parent)
                .Where(p => p.Id.CompareTo(policyId) == 0)
                .FirstOrDefaultAsync();

            return policyEntity == null
                ? null
                : _transformer.ToModel<PolluxPolicy>(policyEntity, TransformCommand.Query);
        }
    }
}
