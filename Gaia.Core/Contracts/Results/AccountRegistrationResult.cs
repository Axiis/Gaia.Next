using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authentication.Contracts.Params;
using Axis.Pollux.Common.Models;
using Gaia.Core.Exceptions;

namespace Gaia.Core.Contracts.Results
{
    public class AccountRegistrationResult: IValidatable
    {
        public MultiFactorAuthenticationToken Token { get; set; }

        public Operation Validate()
        => Operation.Try(() =>
        {
            Token
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState))
                .Validate();
        });
    }
}
