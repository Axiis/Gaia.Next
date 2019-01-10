using System;
using System.Threading.Tasks;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;

namespace Gaia.Services.Queries
{
    public interface IHarvestManagerQueries
    {
        Task<Farmer> GetFarmer(Guid farmerId);
        Task<Farm> GetFarm(Guid farmId);
        Task<HarvestBatch> GetBatch(Guid batchId);
        Task<Harvest> GetHarvest(Guid harvestId);
        Task<FarmProduce> GetFarmProduce(Guid produceId);

        Task<ArrayPage<HarvestBatch>> GetHarvestBatches(Guid farmId, ArrayPageRequest request = null);
        Task<ArrayPage<HarvestBatch>> GetHarvestBatches(ArrayPageRequest request = null);
        Task<ArrayPage<Harvest>> GetHarvests(Guid farmId, Guid batchId, ArrayPageRequest request = null);
        Task<ArrayPage<Harvest>> GetHarvests(Guid farmId, ArrayPageRequest request = null);
        Task<ArrayPage<Harvest>> GetHarvests(ArrayPageRequest request = null);
    }
}
