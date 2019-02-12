using System;
using System.Linq;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Models;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;

namespace Gaia.Core.CustomDataAccessAuth
{
    /// <summary>
    /// Custom data authorization param for evaluating if the current user is one of the admins
    /// in the list provided, or, if the current user has access to all Cooperative data
    /// </summary>
    public class CooperativeAdminDescriptor: IDataAccessDescriptor
    {
        public CooperativeAdminDescriptor()
        { }

        public CooperativeAdminDescriptor(CooperativeAdmin[] admins)
        {
            Admins = admins?
                .Select(ToDescriptor)
                .ToArray();
        }

        public CoopAdmin[] Admins { get; set; }

        public Operation Validate()
        => Operation.Try(() =>
        {
            CustomDataType
                .ThrowIf(
                    string.IsNullOrWhiteSpace,
                    new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));

            Admins
                .Any(IsNotValidDescriptor)
                .ThrowIf(true, new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));
        });

        public string CustomDataType => typeof(CooperativeAdminDescriptor).FullName;

        private CoopAdmin ToDescriptor(CooperativeAdmin admin) => new CoopAdmin
        {
            UserId = admin.User?.Id ?? Guid.Empty,
            CooperativeId = admin.Cooperative?.Id ?? Guid.Empty,
            CooperativeTitle = admin.Cooperative?.Title,
            CooperativeStatus = admin.Cooperative?.Status ?? CooperativeStatus.Disabled
        };

        private bool IsValidDescriptor(CoopAdmin admin)
        {
            return admin is CoopAdmin x
                && x.UserId != Guid.Empty
                && x.CooperativeId != Guid.Empty
                && !string.IsNullOrWhiteSpace(x.CooperativeTitle);
        }

        private bool IsNotValidDescriptor(CoopAdmin admin) => !IsValidDescriptor(admin);



        public class CoopAdmin
        {
            public Guid UserId { get; set; }
            public Guid CooperativeId { get; set; }
            public string CooperativeTitle { get; set; }
            public CooperativeStatus CooperativeStatus { get; set; }
        }
    }
}
