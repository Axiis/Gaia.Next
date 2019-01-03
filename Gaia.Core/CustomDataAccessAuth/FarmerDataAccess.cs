using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts.Params;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;

namespace Gaia.Core.CustomDataAccessAuth
{
    /// <summary>
    /// Custom data authorization param for evaluating if a farmer is modifying his own farm, or if
    /// someone with the right access is doing so.
    /// When specific data are supplied - eg Farm, the policy employs special logic to ensure the data actually belongs
    /// to the Farmer
    /// </summary>
    public class FarmerDataAccess: ICustomAccessDataRoot
    {
        public Farmer Farmer { get; set; }

        /// <summary>
        /// When present, indicates the object (belonging to the farmer) to whom access is being authorized
        /// </summary>
        public Farm Farm { get; set; }

        public string CustomDataType => typeof(FarmerDataAccess).FullName;

        public Operation Validate()
        => Operation.Try(() =>
        {
            Farmer
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));

            //if a farm exists, make sure it's owner is not null
            Farm?.Owner
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));
        });
    }
}
