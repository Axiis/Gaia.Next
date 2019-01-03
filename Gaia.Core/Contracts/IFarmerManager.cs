using System;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface IFarmerManager
    {
        #region Farmer

        /// <summary>
        /// Adds a new farmer instance to the store - the userId provided corresponds to the User Account that
        /// owns the farmer instance.
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <param name="farmer"></param>
        /// <returns></returns>
        Operation<Farmer> CreateFarmer(Guid userAccountId, Farmer farmer);

        /// <summary>
        /// updates the farmer instance
        /// </summary>
        /// <param name="farmer"></param>
        /// <returns></returns>
        Operation<Farmer> UpdateFarmer(Farmer farmer);

        Operation UpdateFarmerStatus(Guid farmerId, FarmerStatus status);

        Operation<Farm> AddFarm(Guid farmerId, Farm farm);
        Operation<Farm> DeleteFarm(Guid farmerId, Guid farmId);


        Operation<ArrayPage<Farmer>> GetFarmers(ArrayPageRequest request = null);

        #endregion
    }
}
