using Axis.Jupiter;
using Axis.Pollux.Authentication.Models;
using Axis.Pollux.Authentication.Services.Queries;
using Gaia.Data.EFCore.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Data.EFCore.Queries.Pollux.Authentication
{
    public class MultiFactorQueries : IMultiFactorQueries
    {
        private readonly DomainStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public MultiFactorQueries(DomainStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<MultiFactorCredential> GetActiveCredential(Guid userId, string eventLabel)
        {
            var data = await _query
                .Query<Entities.Authentication.MultiFactorCredential>(d => d.TargetUser)
                .Where(d => userId.CompareTo(d.TargetUserId) == 0)
                .Where(d => eventLabel == d.EventLabel)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<MultiFactorCredential>(data, TransformCommand.Query);
        }
    }
}
