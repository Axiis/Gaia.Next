using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Models;
using Gaia.Core.Exceptions;
using System;

namespace Gaia.Core.AccessDescriptors
{
    /// <summary>
    /// Custom data authorization param for evaluating if a farmer is modifying his own farm, or if
    /// someone with the right access is doing so.
    /// When specific data are supplied - eg Farm, the policy employs special logic to ensure the data actually belongs
    /// to the Farmer
    /// </summary>
    public class FarmerAccessDescriptor : IDataAccessDescriptor
    {
        public Farmer FarmerData { get; set; }

        /// <summary>
        /// When present, indicates the object (belonging to the farmer) to whom access is being authorized
        /// </summary>
        public Farm FarmData { get; set; }

        public string RootType => typeof(FarmerAccessDescriptor).FullName;


        public FarmerAccessDescriptor(Models.Farmer farmer, Models.Farm farm = null)
        {
            FarmerData = new Farmer
            {
                EnterpriseName = farmer.EnterpriseName,
                Id = farmer.Id,
                UserId = farmer.User.Id
            };

            FarmData = farm == null ? null : new Farm
            {
                Id = farm.Id,
                Owner = farm.Owner == null ? null : new Farmer
                {
                    EnterpriseName = farm.Owner.EnterpriseName,
                    Id = farm.Owner.Id,
                    UserId = farm.Owner.User.Id
                }
            };
        }

        public Operation Validate()
        => Operation.Try(() =>
        {
            FarmerData
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));

            //if a farm exists, make sure it's owner is not null
            FarmData?.Owner
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));
        });


        public class Farmer
        {
            public string EnterpriseName { get; set; }
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
        }

        public class Farm
        {
            public Guid Id { get; set; }
            public Farmer Owner { get; set; }
        }
    }


}
