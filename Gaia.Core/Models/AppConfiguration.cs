using System;
using Axis.Luna.Common;
using Axis.Luna.Common.Contracts;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Models;
using Gaia.Core.Exceptions;

namespace Gaia.Core.Models
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class AppConfiguration: BaseModel<Guid>, IDataItem
    {
        public string Name { get; set; }
        public string Scope { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }

        public AppConfiguration Parent { get; set; }


        public void Initialize(string[] tuple)
        {
            if (tuple == null || tuple.Length != 4)
                throw new GaiaException(PolluxErrorCodes.InvalidArgument);

            Name = tuple[0];
            Scope = tuple[1];

            Enum.TryParse(tuple[2], out CommonDataType commonDataType)
                .ThrowIf(false, new GaiaException(PolluxErrorCodes.InvalidArgument));
            Type = commonDataType;

            Data = tuple[3];
        }

        public string[] Tupulize() => new[]
        {
            Name,
            Scope,
            Type.ToString(),
            Data
        };

        public override Operation Validate()
        => base.Validate().Then(() =>
        {
            if(string.IsNullOrWhiteSpace(Name)
            || string.IsNullOrWhiteSpace(Data))
                throw new GaiaException(PolluxErrorCodes.ModelValidationError);
        });
    }
}
