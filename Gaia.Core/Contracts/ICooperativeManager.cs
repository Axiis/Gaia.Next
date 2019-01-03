using System;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface ICooperativeManager
    {
        Operation<Cooperative> CreateCooperative(Cooperative cooperative);
        Operation<Cooperative> UpdateCooperative(Cooperative cooperative);
        Operation UpdateCooperativeStatus(Guid cooperativeId, CooperativeStatus status);

        Operation AddAdmin(Guid cooperativeId, Guid userId);
        Operation RemoveAdmin(Guid cooperativeId, Guid userId);

        Operation AddFarm(Guid cooperativeId, Guid farmId);
        Operation RemoveFarm(Guid cooperativeId, Guid farmId);

        Operation<Cooperative> GetCooperative(Guid cooperativeId);

        Operation<ArrayPage<Cooperative>> GetAdminCooperatives(Guid userId, ArrayPageRequest request = null);
        Operation<ArrayPage<Cooperative>> GetFarmerCooperatives(Guid farmerId, ArrayPageRequest request = null);
        Operation<ArrayPage<Cooperative>> GetAllCooperatives(ArrayPageRequest request = null);

        Operation<ArrayPage<Farmer>> GetRegisteredFarmers(Guid cooperativeId, ArrayPageRequest request = null);
        Operation<ArrayPage<Farm>> GetRegisteredFarms(Guid cooperativeId, ArrayPageRequest request = null);
    }
}
