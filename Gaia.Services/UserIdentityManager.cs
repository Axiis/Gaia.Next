using System;
using Axis.Jupiter;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts;
using Gaia.Core.Contracts;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Services.Queries;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class UserIdentityManager: IUserIdentityManager
    {
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAuth;
        private readonly IUserNameValidator _userNameValidator;
        private readonly IUserIdentityManagerQueries _queries;


        public UserIdentityManager(
            IUserNameValidator usernameValidator,
            IDataAccessAuthorizer dataAuthorizer,
            IUserIdentityManagerQueries queries,
            StoreProvider storeProvider)
        {
            ThrowNullArguments(
                () => usernameValidator,
                () => dataAuthorizer,
                () => queries,
                () => storeProvider);

            _storeProvider = storeProvider;
            _dataAuth = dataAuthorizer;
            _queries = queries;
            _userNameValidator = usernameValidator;
        }


        public Operation<UserIdentity> CreateIdentity(string userName, Guid userId)
        => Operation.Try(async () =>
        {
            //verify the parameters
            userId.ThrowIf(
                default(Guid), 
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            userName.ThrowIf(
                string.IsNullOrWhiteSpace,
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //data access authorization
            await _dataAuth.AuthorizeAccess(typeof(UserIdentity).FullName);

            //get the user object
            var user = (await _queries
                .GetUser(userId))
                .ThrowIfNull(new GaiaException(ErrorCodes.DomainLogicError));

            //make sure this user doesn't already have an identity object
            (await _queries
                .GetUserIdentity(user.Id))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //make sure this user name is unique
            (await _queries
                .GetUserIdentity(userName))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            var userIdentity = new UserIdentity
            {
                Owner = user,
                UserName = await _userNameValidator.Validate(userName)
            };

            var storeCommand = _storeProvider.CommandFor(typeof(UserIdentity).FullName);
            return (await storeCommand
                .Add(userIdentity))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<UserIdentity> UpdateIdentity(Guid userIdentityId, string newUserName)
        => Operation.Try(async () =>
        {
            //validate parameters
            userIdentityId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            await _userNameValidator.Validate(newUserName);

            var userIdentity = (await _queries
                .GetUserIdentityById(userIdentityId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check access
            await _dataAuth.AuthorizeAccess(
                typeof(UserIdentity).FullName,
                userIdentity.Owner.Id,
                userIdentityId.ToString());

            //make sure new user name is unique
            (await _queries
                .GetUserIdentity(newUserName))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //make modifications
            userIdentity.UserName = newUserName;

            var storeCommand = _storeProvider.CommandFor(typeof(UserIdentity).FullName);
            return (await storeCommand
                .Update(userIdentity))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Guid> GetUserIdForIdentity(string userName)
        => Operation.Try(async () =>
        {
            //validate parameters
            userName.ThrowIf(
                string.IsNullOrWhiteSpace,
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var userIdentity = (await _queries
                .GetUserIdentity(userName))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check access
            await _dataAuth.AuthorizeAccess(
                typeof(UserIdentity).FullName,
                userIdentity.Owner.Id,
                userIdentity.Id.ToString());

            return userIdentity.Owner.Id;
        });

        public Operation<UserIdentity> GetIdentity(string userName)
        => Operation.Try(async () =>
        {
            //validate parameters
            userName.ThrowIf(
                string.IsNullOrWhiteSpace,
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var userIdentity = (await _queries
                .GetUserIdentity(userName))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check access
            await _dataAuth.AuthorizeAccess(
                typeof(UserIdentity).FullName,
                userIdentity.Owner.Id,
                userIdentity.Id.ToString());

            return userIdentity;
        });

        public Operation<UserIdentity> GetIdentity(Guid identityId)
        => Operation.Try(async () =>
        {
            //validate parameters
            identityId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var userIdentity = (await _queries
                .GetUserIdentityById(identityId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check access
            await _dataAuth.AuthorizeAccess(
                typeof(UserIdentity).FullName,
                userIdentity.Owner.Id,
                userIdentity.Id.ToString());

            return userIdentity;
        });
    }
}
