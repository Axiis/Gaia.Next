using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Axis.Pollux.Common.Utils;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;

namespace Gaia.Services.Queries
{
    public interface ICooperativeManagerQueries
    {
        Task<Cooperative> GetCooperative(Guid cooperativeId);
        Task<ArrayPage<CooperativeAdmin>> GetAdmins(Guid cooperativeId, ArrayPageRequest request = null);
        Task<ArrayPage<Cooperative>> GetAdminCooperatives(Guid userId, ArrayPageRequest request = null);
        Task<ArrayPage<Cooperative>> GetFarmerCooperatives(Guid farmerId, ArrayPageRequest request = null);
        Task<ArrayPage<Cooperative>> GetAllCooperatives(ArrayPageRequest request = null);

        Task<Farm> GetFarm(Guid farmId);
        Task<CooperativeFarm> GetCooperativeFarm(Guid cooperativeId, Guid farmId);

        Task<User> GetUser(Guid userId);

        Task<ArrayPage<Farmer>> GetRegisteredFarmers(Guid cooperativeId, ArrayPageRequest request = null);
        Task<ArrayPage<Farm>> GetRegisteredFarms(Guid cooperativeId, ArrayPageRequest request = null);
    }
}
