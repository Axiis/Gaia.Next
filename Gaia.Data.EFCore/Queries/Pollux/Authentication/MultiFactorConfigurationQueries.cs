using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Axis.Pollux.Authentication.Models;
using Axis.Pollux.Authentication.Services.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Data.EFCore.Queries.Pollux.Authentication
{
    public class MultiFactorConfigurationQueries : IMultiFactorConfigurationQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public MultiFactorConfigurationQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<MultiFactorEventConfiguration> GetMultiFactorConfiguration(string eventLabel)
        {
            var data = await _query
                .Query<Entities.Authentication.MultiFactorEventConfiguration>()
                .Where(d => eventLabel == d.EventLabel)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<MultiFactorEventConfiguration>(data, TransformCommand.Query);
        }
    }
}
