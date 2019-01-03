using System;
using System.Collections.Generic;
using System.Text;

namespace Gaia.Core.Exceptions
{
    public static class ErrorCodes
    {
        /// <summary>
        /// When common things like null pointer issues are found
        /// </summary>
        public static readonly string GeneralError = "Gaia.Error.0";

        /// <summary>
        /// When a parameters passed into a domain operation causes a breach in domain rules/logic. This is typically
        /// the error code used for domain related errors
        /// </summary>
        public static readonly string DomainLogicError = "Gaia.Error.1";

        /// <summary>
        /// When the result of a store command is null or invalid
        /// </summary>
        public static readonly string InvalidStoreCommandResult = "Gaia.Error.2";

        /// <summary>
        /// When the result of a store query is null or invalid
        /// </summary>
        public static readonly string InvalidStoreQueryResult = "Gaia.Error.3";
    }
}
