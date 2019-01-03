using System;
using System.Collections.Generic;
using System.Text;
using Axis.Pollux.Common.Exceptions;

namespace Gaia.Core.Exceptions
{
    public class GaiaException: Exception, IExceptionContract
    {
        public string Code { get; }
        public object Info { get; }

        public GaiaException(string code, object data = null, Exception innerException = null)
        : base(null, innerException)
        {
            Code = code;
            Info = data;
        }
    }
}
