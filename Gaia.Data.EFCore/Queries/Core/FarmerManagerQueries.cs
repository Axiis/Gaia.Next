using static Axis.Luna.Extensions.ExceptionExtension;

using Axis.Jupiter;
using Axis.Pollux.Common.Utils;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;
using Gaia.Services.Queries;
using System;
using System.Threading.Tasks;
using Axis.Jupiter.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Gaia.Data.EFCore.Queries.Core
{
    public class FarmerManagerQueries : IFarmerManagerQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public FarmerManagerQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<ArrayPage<Farmer>> GetAllFarmers(ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Farmer>(d => d.User, d => d.Farms);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Farmer>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Farmer, Farmer>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<Farm> GetFarm(Guid farmId)
        {
            var farmEntity = await _query
                .Query<Entities.Farm>()
                .Where(farm => farm.Id.CompareTo(farmId) == 0)
                .FirstOrDefaultAsync();

            return farmEntity == null
                ? null
                : _transformer.ToModel<Farm>(farmEntity, TransformCommand.Query);
        }

        public async Task<Farmer> GetFarmer(Guid farmerId)
        {
            var farmerEntity = await _query
                .Query<Entities.Farmer>()
                .Where(farmer => farmer.Id.CompareTo(farmerId) == 0)
                .FirstOrDefaultAsync();

            return farmerEntity == null
                ? null
                : _transformer.ToModel<Farmer>(farmerEntity, TransformCommand.Query);
        }

        public async Task<User> GetUser(Guid userId)
        {
            var userEntity = await _query
                .Query<Entities.Identity.User>()
                .Where(user => user.Id.CompareTo(userId) == 0)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<User>(userEntity, TransformCommand.Query);
        }
    }
}
