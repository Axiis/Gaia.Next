using System.Linq;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts.Params;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;

namespace Gaia.Core.CustomDataAccessAuth
{
    /// <summary>
    /// Custom data authorization param for evaluating if the current user is one of the admins
    /// in the list provided, or, if the current user has access to all Cooperative data
    /// </summary>
    public class CooperativeAdminDataAccess: ICustomAccessDataRoot
    {
        public CooperativeAdmin[] Admins { get; set; }

        public Operation Validate()
        => Operation.Try(() =>
        {
            CustomDataType
                .ThrowIf(
                    string.IsNullOrWhiteSpace,
                    new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));
        });

        public string CustomDataType => typeof(CooperativeAdminDataAccess).FullName;

        public object CompressObjectGraph() => new
        {
            CustomDataType,
            Admins = Admins
                .Select(admin => new
                {
                    User = new {admin.User.Id},
                    Cooperative = new
                    {
                        admin.Cooperative.Id,
                        admin.Cooperative.Title
                    }
                })
                .ToArray()
        };
    }
}
