using Gaia.Core.Models;
using System;
using System.Threading.Tasks;
using Axis.Pollux.Common.Utils;

namespace Gaia.Services.Queries
{
    public interface IFarmManagerQueries
    {
        Task<Farmer> GetFarmer(Guid farmerId);
        Task<Farm> GetFarm(Guid farmId);
        Task<FarmProduce> GetFarmProduce(Guid farmProduceId);
        Task<ProductCategory> GetProductCategory(Guid farmId, Guid productId);
        Task<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null);
    }
}
