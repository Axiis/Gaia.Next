using System;
using Axis.Luna.Operation;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface IUserIdentityManager
    {
        Operation<UserIdentity> CreateIdentity(string userName, Guid userId);
        Operation<UserIdentity> UpdateIdentity(Guid userIdentity, string newUserName);

        Operation<Guid> GetUserIdForIdentity(string userName);
        Operation<UserIdentity> GetIdentity(string userName);
        Operation<UserIdentity> GetIdentity(Guid identityId);
    }

    public interface IUserNameValidator
    {
        /// <summary>
        /// Returns a faulted operation if the user name is not in the correct format, else
        /// returns the original user-name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Operation<string> Validate(string userName);
    }
}
