using static Axis.Luna.Extensions.ExceptionExtension;

using Gaia.Services.Queries;
using System;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;
using System.Threading.Tasks;
using Axis.Jupiter.EFCore;
using Axis.Jupiter;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Queries.Core
{
    public class FarmManagerQueries : IFarmManagerQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public FarmManagerQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
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

        public async Task<FarmProduce> GetFarmProduce(Guid farmProduceId)
        {
            var productEntity = await _query
                .Query<Entities.FarmProduce>()
                .Where(product => product.Id.CompareTo(farmProduceId) == 0)
                .FirstOrDefaultAsync();

            return productEntity == null
                ? null
                : _transformer.ToModel<FarmProduce>(productEntity, TransformCommand.Query);
        }

        public async Task<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Farm>(
                    d => d.Owner, 
                    d => d.Cooperatives,
                    d => d.Harvests,
                    d => d.Products);

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

        public async Task<ProductCategory> GetProductCategory(Guid farmId, Guid productId)
        {
            var farmerEntity = await _query
                .Query<Entities.ProductCategory>(
                    cat => cat.Farm,
                    cat => cat.FarmProduce)
                .Where(cat => farmId.CompareTo(cat.FarmId) == 0)
                .Where(cat => productId.CompareTo(cat.FarmProduceId) == 0)
                .FirstOrDefaultAsync();

            return farmerEntity == null
                ? null
                : _transformer.ToModel<ProductCategory>(farmerEntity, TransformCommand.Query);
        }
    }
}
