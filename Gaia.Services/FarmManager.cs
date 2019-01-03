using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts;
using Gaia.Core.Models;
using Gaia.Core.Utils;

namespace Gaia.Services
{
    public class FarmManager: IFarmManager
    {
        public Operation<Farm> CreateFarm(Guid farmer, Farm farm)
        {
            throw new NotImplementedException();
        }

        public Operation<Farm> UpdateFarm(Farm farm)
        {
            throw new NotImplementedException();
        }

        public Operation<Farm> UpdateFarmGeoAreaData(Guid farmId, GeoPosition[] updatedGeoArea)
        {
            throw new NotImplementedException();
        }

        public Operation AddProductCategory(Guid farmId, Guid farmProductId)
        {
            throw new NotImplementedException();
        }

        public Operation RemoveProductCategory(Guid farmId, Guid farmProductId)
        {
            throw new NotImplementedException();
        }

        public Operation<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null)
        {
            throw new NotImplementedException();
        }
    }
}
