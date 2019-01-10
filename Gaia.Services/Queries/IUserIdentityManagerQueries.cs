using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;

namespace Gaia.Services.Queries
{
    public interface IUserIdentityManagerQueries
    {
        Task<UserIdentity> GetUserIdentity(string userName);
        Task<UserIdentity> GetUserIdentity(Guid ownerUserId);
        Task<UserIdentity> GetUserIdentityById(Guid userIdentityId);
        Task<User> GetUser(Guid userId);
    }
}
