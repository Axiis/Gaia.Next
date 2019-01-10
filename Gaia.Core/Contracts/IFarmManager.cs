using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;
using Gaia.Core.Utils;

namespace Gaia.Core.Contracts
{
    public interface IFarmManager
    { 
        Operation<Farm> CreateFarm(Guid farmerId, Farm farm);

        Operation<Farm> UpdateFarm(Farm farm);

        Operation<Farm> UpdateFarmGeoAreaData(Guid farmId, GeoPosition[] updatedGeoArea);

        Operation AddProductCategory(Guid farmId, Guid farmProductId);
        Operation RemoveProductCategory(Guid farmId, Guid farmProductId);


        Operation<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null);
    }
}
