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
}
