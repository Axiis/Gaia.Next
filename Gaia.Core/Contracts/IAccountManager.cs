using System;
using System.Collections.Generic;
using System.Text;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts.Params;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface IAccountManager
    {
        Operation RegisterCooperative(CooperativeOnlyRegistrationInfo info);
        Operation RegisterUserAndCooperative(UserAndCooperativeRegistrationInfo info);

        /// <summary>
        /// Create the farmer object, and a user object for it.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation RegisterFarmer(FarmerRegistrationInfo info);

        /// <summary>
        /// Registers the the farmer object, a user object for it, a farm using the given name, and then
        /// assigns the farms to the given cooperative.
        /// </summary>
        /// <param name="cooperativeId"></param>
        /// <param name="farms">string array containing the farm names to register. MUST NOT be null</param>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation RegisterCooperativeFarmer(Guid cooperativeId, string[] farms, FarmerRegistrationInfo info);
    }
}
