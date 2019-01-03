using System;
using System.Threading.Tasks;
using Axis.Pollux.Common.Utils;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;

namespace Gaia.Services.Queries
{
    public interface IFarmerManagerQueries
    {
        Task<User> GetUser(Guid userId);
        Task<Farmer> GetFarmer(Guid farmerId);
        Task<Farm> GetFarm(Guid farmId);

        Task<ArrayPage<Farmer>> GetAllFarmers(ArrayPageRequest request = null);
    }
}
