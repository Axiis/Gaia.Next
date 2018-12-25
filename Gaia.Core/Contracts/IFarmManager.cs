using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;
using Gaia.Core.Utils;

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

        Operation UpdateFarmerStatus(Guid farmerId, int status);


        Operation<ArrayPage<Farmer>> GetFarmers(ArrayPageRequest request = null);

        #endregion
    }

    public interface IFarmManager
    { 
        Operation<Farm> CreateFarm(Guid farmer, Farm farm);

        Operation<Farm> UpdateFarm(Farm farm);

        Operation<Farm> UpdateFarmGeoAreaData(Guid farmId, GeoPosition[] updatedGeoArea);

        Operation AddProductCategory(Guid farmId, Guid farmProductId);
        Operation RemoveProductCategory(Guid farmId, Guid farmProductId);


        Operation<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null);
    }

    public interface IHarvestManager
    {
        Operation<HarvestBatch> CreateHarvestBatch(Guid farmId, HarvestBatch batch);

        /// <summary>
        /// supports modification of title and date
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        Operation<HarvestBatch> UpdateHarvestBatch(HarvestBatch batch);
    }
}
