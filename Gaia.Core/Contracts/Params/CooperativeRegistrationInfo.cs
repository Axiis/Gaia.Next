using System;
using System.Linq;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Exceptions;
using Axis.Pollux.Common.Models;

namespace Gaia.Core.Contracts.Params
{
    public class CooperativeOnlyRegistrationInfo : IValidatable
    {
        /// <summary>
        /// Title of the cooperative
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Guid of the initial admin
        /// </summary>
        public Guid AdminUserId { get; set; }


        public Operation Validate()
        => Operation.Try(() =>
        {
            if (string.IsNullOrWhiteSpace(Title)
                || AdminUserId == default(Guid))
                throw new CommonException(ErrorCodes.InvalidContractParamState);
        });
    }

    public class UserAndCooperativeRegistrationInfo : IValidatable
    {
        /// <summary>
        /// Title of the cooperative
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The email of the initial admin for the cooperative account, used as the User Name for the user too. (can be changed)
        /// </summary>
        public string AdminEmail { get; set; }

        /// <summary>
        /// Password for the user account
        /// </summary>
        public string AdminPassword { get; set; }


        public Operation Validate()
        => Operation.Try(() =>
        {
            new[] {Title, AdminEmail, AdminPassword}
                .Any(string.IsNullOrWhiteSpace)
                .ThrowIf(true, new CommonException(ErrorCodes.InvalidContractParamState));
        });
    }
}
