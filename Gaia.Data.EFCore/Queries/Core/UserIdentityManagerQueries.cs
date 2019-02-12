using static Axis.Luna.Extensions.ExceptionExtension;

using Axis.Pollux.Identity.Models;
using Gaia.Core.Models;
using Gaia.Services.Queries;
using System;
using System.Threading.Tasks;
using Axis.Jupiter.EFCore;
using Axis.Jupiter;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Queries.Core
{
    public class UserIdentityManagerQueries : IUserIdentityManagerQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public UserIdentityManagerQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<User> GetUser(Guid userId)
        {
            var userEntity = await _query
                .Query<Entities.Identity.User>()
                .Where(user => user.Id.CompareTo(userId) == 0)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<User>(userEntity, TransformCommand.Query);
        }

        public async Task<UserIdentity> GetUserIdentity(string userName)
        {
            var userEntity = await _query
                .Query<Entities.UserIdentity>(ui => ui.Owner)
                .Where(user => user.UserName == userName)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<UserIdentity>(userEntity, TransformCommand.Query);
        }

        public async Task<UserIdentity> GetUserIdentity(Guid ownerUserId)
        {
            var userEntity = await _query
                .Query<Entities.UserIdentity>(ui => ui.Owner)
                .Where(user => ownerUserId.CompareTo(user.OwnerId) == 0)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<UserIdentity>(userEntity, TransformCommand.Query);
        }

        public async Task<UserIdentity> GetUserIdentityById(Guid userIdentityId)
        {
            var userEntity = await _query
                .Query<Entities.UserIdentity>(ui => ui.Owner)
                .Where(userIdentity => userIdentityId.CompareTo(userIdentity.Id) == 0)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<UserIdentity>(userEntity, TransformCommand.Query);
        }
    }
}
