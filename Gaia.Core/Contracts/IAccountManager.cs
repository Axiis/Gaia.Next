using System;
using Axis.Luna.Operation;
using Axis.Pollux.Authentication.Contracts.Params;
using Gaia.Core.Contracts.Params;
using Gaia.Core.Contracts.Results;

namespace Gaia.Core.Contracts
{
    public interface IAccountManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation RegisterCooperative(CooperativeOnlyRegistrationInfo info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation<AccountRegistrationResult> RegisterUserAndCooperative(UserAndCooperativeRegistrationInfo info);

        /// <summary>
        /// Create the farmer object, and a user object for it.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation<AccountRegistrationResult> RegisterFarmer(FarmerRegistrationInfo info);

        /// <summary>
        /// Registers the the farmer object, a user object for it, a farm using the given name, and then
        /// assigns the farms to the given cooperative.
        /// </summary>
        /// <param name="cooperativeId"></param>
        /// <param name="farms">string array containing the farm names to register. MUST NOT be null</param>
        /// <param name="info"></param>
        /// <returns></returns>
        Operation<AccountRegistrationResult> RegisterCooperativeFarmer(Guid cooperativeId, string[] farms, FarmerRegistrationInfo info);

        /// <summary>
        /// Calls to register cooperatives along with users will generate multi-factor tokens whose validation callback
        /// is this method
        /// </summary>
        /// <param name="authInfo"></param>
        /// <returns></returns>
        Operation ValidateCooperativeAccountRegistration(MultiFactorAuthenticationInfo authInfo);

        /// <summary>
        /// Calls to register farmers will generate multi-factor tokens whose validation callback
        /// is this method
        /// </summary>
        /// <param name="authInfo"></param>
        /// <returns></returns>
        Operation ValidateFarmerAccountRegistration(MultiFactorAuthenticationInfo authInfo);
    }
}
