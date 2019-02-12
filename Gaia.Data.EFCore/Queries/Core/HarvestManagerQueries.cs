using static Axis.Luna.Extensions.ExceptionExtension;

using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Gaia.Services.Queries;
using System;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Queries.Core
{
    public class HarvestManagerQueries: IHarvestManagerQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public HarvestManagerQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<HarvestBatch> GetBatch(Guid batchId)
        {
            var batchEntity = await _query
                .Query<Entities.HarvestBatch>(
                    h => h.Farm,
                    h => h.Harvests)
                .Where(h => h.Id.CompareTo(batchId) == 0)
                .FirstOrDefaultAsync();

            return batchEntity == null
                ? null
                : _transformer.ToModel<HarvestBatch>(batchEntity, TransformCommand.Query);
        }

        public async Task<Farm> GetFarm(Guid farmId)
        {
            var farmEntity = await _query
                .Query<Entities.Farm>(
                    f => f.Owner,
                    f => f.Cooperatives,
                    f => f.Harvests,
                    f => f.Products)
                .Where(f => f.Id.CompareTo(farmId) == 0)
                .FirstOrDefaultAsync();

            return farmEntity == null
                ? null
                : _transformer.ToModel<Farm>(farmEntity, TransformCommand.Query);
        }

        public async Task<Farmer> GetFarmer(Guid farmerId)
        {
            var farmerEntity = await _query
                .Query<Entities.Farmer>(
                    f => f.User,
                    f => f.Farms)
                .Where(f => f.Id.CompareTo(farmerId) == 0)
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

        public async Task<Harvest> GetHarvest(Guid harvestId)
        {
            var harvestEntity = await _query
                .Query<Entities.Harvest>(
                    h => h.Batch,
                    h  => h.Produce)
                .Where(h => h.Id.CompareTo(harvestId) == 0)
                .FirstOrDefaultAsync();

            return harvestEntity == null
                ? null
                : _transformer.ToModel<Harvest>(harvestEntity, TransformCommand.Query);
        }

        public async Task<ArrayPage<HarvestBatch>> GetHarvestBatches(Guid farmId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.HarvestBatch>(
                    h => h.Farm,
                    h => h.Harvests)
                .Where(h => farmId.CompareTo(h.FarmId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<HarvestBatch>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.HarvestBatch, HarvestBatch>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<HarvestBatch>> GetHarvestBatches(ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.HarvestBatch>(
                    h => h.Farm,
                    h => h.Harvests);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<HarvestBatch>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.HarvestBatch, HarvestBatch>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Harvest>> GetHarvests(Guid farmId, Guid batchId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Harvest>(
                    h => h.Batch,
                    h => h.Produce)
                .Where(h => batchId.CompareTo(h.HarvestBatchId) == 0)
                .Where(h => farmId.CompareTo(h.Batch.FarmId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Harvest>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Harvest, Harvest>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Harvest>> GetHarvests(Guid farmId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Harvest>(
                    h => h.Batch,
                    h => h.Produce)
                .Where(h => farmId.CompareTo(h.Batch.FarmId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Harvest>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Harvest, Harvest>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Harvest>> GetHarvests(ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Harvest>(
                    h => h.Batch,
                    h => h.Produce);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Harvest>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Harvest, Harvest>(
                    _transformer,
                    TransformCommand.Query)
            };
        }
    }
}
