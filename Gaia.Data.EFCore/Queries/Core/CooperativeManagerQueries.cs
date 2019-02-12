using static Axis.Luna.Extensions.ExceptionExtension;

using Axis.Jupiter;
using Axis.Pollux.Common.Utils;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;
using Gaia.Services.Queries;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Axis.Jupiter.EFCore;

namespace Gaia.Data.EFCore.Queries.Core
{
    public class CooperativeManagerQueries : ICooperativeManagerQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public CooperativeManagerQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<ArrayPage<Cooperative>> GetAdminCooperatives(Guid userId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.CooperativeAdmin>(c => c.Cooperative.Farms)
                .Where(c => userId.CompareTo(c.UserId) == 0)
                .Select(c => c.Cooperative);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Cooperative>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Cooperative, Cooperative>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<CooperativeAdmin>> GetAdmins(Guid cooperativeId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.CooperativeAdmin>(c => c.Cooperative, c => c.User)
                .Where(c => cooperativeId.CompareTo(c.CooperativeId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<CooperativeAdmin>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.CooperativeAdmin, CooperativeAdmin>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Cooperative>> GetAllCooperatives(ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Cooperative>();

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Cooperative>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Cooperative, Cooperative>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<Cooperative> GetCooperative(Guid cooperativeId)
        {
            var cooperative = await _query
                .Query<Entities.Cooperative>(c => c.Farms, c => c.Admins)
                .Where(c => cooperativeId.CompareTo(c.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<Cooperative>(cooperative, TransformCommand.Query);
        }

        public async Task<CooperativeFarm> GetCooperativeFarm(Guid cooperativeId, Guid farmId)
        {
            var cooperative = await _query
                .Query<Entities.CooperativeFarm>(c => c.Farm, c => c.Cooperative)
                .Where(c => cooperativeId.CompareTo(c.Id) == 0)
                .Where(c => farmId.CompareTo(farmId) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<CooperativeFarm>(cooperative, TransformCommand.Query);
        }

        public async Task<Farm> GetFarm(Guid farmId)
        {
            var cooperative = await _query
                .Query<Entities.Farm>(
                    c => c.Cooperatives,
                    c => c.Harvests,
                    c => c.Products)
                .Where(c => c.Id.CompareTo(farmId) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<Farm>(cooperative, TransformCommand.Query);
        }

        public async Task<ArrayPage<Cooperative>> GetFarmerCooperatives(Guid farmerId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery =
                from f in _query.Query<Entities.Farmer>()
                join a in _query.Query<Entities.CooperativeAdmin>() on f.UserId equals a.UserId
                join c in _query.Query<Entities.Cooperative>() on a.CooperativeId equals c.Id
                where farmerId.CompareTo(f.Id) == 0
                select c;

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Cooperative>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Cooperative, Cooperative>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Farmer>> GetRegisteredFarmers(Guid cooperativeId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery =
                from c in _query.Query<Entities.Cooperative>()
                join a in _query.Query<Entities.CooperativeAdmin>() on c.Id equals a.CooperativeId
                join f in _query.Query<Entities.Farmer>() on a.UserId equals f.UserId
                where cooperativeId.CompareTo(c.Id) == 0
                select f;

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

        public async Task<ArrayPage<Farm>> GetRegisteredFarms(Guid cooperativeId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Cooperative>(
                    c => c.Farms.Select(f => f.Harvests),
                    c => c.Farms.Select(f => f.Products),
                    c => c.Farms.Select(f => f.Cooperatives))
                .Where(c => cooperativeId.CompareTo(c.Id) == 0)
                .SelectMany(c => c.Farms);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Farm>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Farm, Farm>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<User> GetUser(Guid userId)
        {
            var cooperative = await _query
                .Query<Entities.Identity.User>()
                .Where(c => c.Id.CompareTo(userId) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<User>(cooperative, TransformCommand.Query);
        }
    }
}
