using System;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authentication.Contracts;
using Axis.Pollux.Authentication.Contracts.Params;
using Axis.Pollux.Authentication.Models;
using Axis.Pollux.Authorization.Contracts;
using Axis.Pollux.Identity.Contracts;
using Axis.Pollux.Identity.Models;
using Gaia.Core.Contracts;
using Gaia.Core.Contracts.Params;
using Gaia.Core.Contracts.Results;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Core.Utils;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    public class AccountManager: IAccountManager
    {
        #region Fields
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAccessAuthorizer;
        private readonly ICredentialManager _credentialManager;
        private readonly IUserManager _userManager;
        private readonly IUserIdentityManager _userIdentityManager;
        private readonly ICooperativeManager _cooperativeManager;
        private readonly IContactDataManager _contactManager;
        private readonly IFarmerManager _farmerManager;
        private readonly IMultiFactorAuthenticator _multiFactorAuth;
        #endregion
        
        public AccountManager(
            IDataAccessAuthorizer dataAuthorizer,
            IUserManager userManager,
            IContactDataManager contactManager,
            ICredentialManager credentialManager,
            ICooperativeManager cooperativeManager,
            IFarmerManager farmerManager,
            IUserIdentityManager userIdentityManager,
            IMultiFactorAuthenticator multiFactorAuthenticator,
            StoreProvider storeProvider)
        {
            ThrowNullArguments(
                nameof(dataAuthorizer).ObjectPair(dataAuthorizer),
                nameof(userManager).ObjectPair(userManager),
                nameof(contactManager).ObjectPair(contactManager),
                nameof(credentialManager).ObjectPair(credentialManager),
                nameof(cooperativeManager).ObjectPair(cooperativeManager),
                nameof(farmerManager).ObjectPair(farmerManager),
                nameof(userIdentityManager).ObjectPair(multiFactorAuthenticator),
                nameof(multiFactorAuthenticator).ObjectPair(multiFactorAuthenticator),
                nameof(storeProvider).ObjectPair(storeProvider));

            _storeProvider = storeProvider;
            _dataAccessAuthorizer = dataAuthorizer;
            _userManager = userManager;
            _userIdentityManager = userIdentityManager;
            _credentialManager = credentialManager;
            _cooperativeManager = cooperativeManager;
            _contactManager = contactManager;
            _farmerManager = farmerManager;
            _multiFactorAuth = multiFactorAuthenticator;
        }

        public Operation RegisterCooperative(CooperativeOnlyRegistrationInfo info)
        => Operation.Try(async () =>
        {
            //validate parameters
            await info
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState))
                .Validate();

            //make sure the admin user exists
            var adminUser = (await _userManager
                .GetUser(info.AdminUserId))
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState));
            
            //create the cooperative
            var cooperative = await _cooperativeManager.CreateCooperative(new Cooperative
            {
                Title = info.Title
            });

            //make the user an admin for the cooperative
            await _cooperativeManager.AddAdmin(cooperative.Id, adminUser.Id);
        });

        public Operation<AccountRegistrationResult> RegisterUserAndCooperative(UserAndCooperativeRegistrationInfo info)
        => Operation.Try(async () =>
        {
            //validate parameters
            await info
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState))
                .Validate();

            //make sure a user with the given name doesn't already exist
            (await _userIdentityManager
                .GetIdentity(info.AdminEmail))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //create the user
            var user = await _userManager.CreateUser(Constants.UserStatus_Unverified);

            //create the identity
            //since we require the initial user name to be the email, make sure it's a valid email address
            await _userIdentityManager.CreateIdentity(info.AdminEmail, user.Id);

            //add the password credential
            await _credentialManager.AddCredential(user.Id, new Credential
            {
                Visibility = CredentialVisibility.Secret,
                Status = Constants.CredentialStatus_Active,
                Uniqueness = Uniqueness.None,
                Name =  Constants.CredentialType_Password,
                Data = info.AdminPassword
            });

            //add the contact detail for the email
            var contact = await _contactManager
                .AddContactData(user.Id, new ContactData
                {
                    Data = info.AdminEmail,
                    Channel = Constants.ContactChannel_Email,
                    Status = Constants.ContactStatus_Active
                });

            //create the cooperative
            var cooperative = await _cooperativeManager.CreateCooperative(new Cooperative
            {
                Title = info.Title
            });

            //make the user an admin for the cooperative
            await _cooperativeManager.AddAdmin(cooperative.Id, user.Id);

            //request account verification multi-factor authentication
            var token = await _multiFactorAuth.RequestMultiFactorToken(
                user.Id,
                Constants.MultiFactorEventLabels_CooperativeAdminAccountVerification,
                contact.Id);

            if(token == null)
                throw new GaiaException(ErrorCodes.GeneralError);

            else
                return new AccountRegistrationResult {Token = token};
        });

        public Operation<AccountRegistrationResult> RegisterFarmer(FarmerRegistrationInfo info)
        => Operation.Try(async () =>
        {
            //validate parameters
            await info
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState))
                .Validate();

            //make sure a user with the given name doesn't already exist
            (await _userIdentityManager
                .GetIdentity(info.AccountEmail))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //create the user
            var user = await _userManager.CreateUser(Constants.UserStatus_Unverified);

            //create the identity
            //since we require the initial user name to be the email, make sure it's a valid email address
            await _userIdentityManager.CreateIdentity(info.AccountEmail, user.Id);

            //add the password credential
            await _credentialManager.AddCredential(user.Id, new Credential
            {
                Visibility = CredentialVisibility.Secret,
                Status = Constants.CredentialStatus_Active,
                Uniqueness = Uniqueness.None,
                Name = Constants.CredentialType_Password,
                Data = info.AccountPassword
            });

            //add the contact detail for the email
            var contact = await _contactManager.AddContactData(user.Id, new ContactData
            {
                Data = info.AccountEmail,
                Channel = Constants.ContactChannel_Email,
                Status = Constants.ContactStatus_Active
            });

            //create the farmer
            await _farmerManager.CreateFarmer(user.Id, new Farmer
            {
                EnterpriseName = info.EnterpriseName
            });

            //request account verification multi-factor authentication
            var token = await _multiFactorAuth.RequestMultiFactorToken(
                user.Id,
                Constants.MultiFactorEventLabels_FarmerAccountVerification,
                contact.Id);

            if (token == null)
                throw new GaiaException(ErrorCodes.GeneralError);

            else
                return new AccountRegistrationResult { Token = token };
        });

        public Operation<AccountRegistrationResult> RegisterCooperativeFarmer(
            Guid cooperativeId, 
            string[] farms, 
            FarmerRegistrationInfo info)
        => Operation.Try(async () =>
        {
            //validate parameters
            await info
                .ThrowIfNull(new GaiaException(Axis.Pollux.Common.Exceptions.ErrorCodes.InvalidContractParamState))
                .Validate();

            //get the cooperative
            var coop = (await _cooperativeManager
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.GeneralError));

            //make sure a user with the given name doesn't already exist
            (await _userIdentityManager
                .GetIdentity(info.AccountEmail))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //create the user
            var user = await _userManager.CreateUser(Constants.UserStatus_Unverified);

            //create the identity
            //since we require the initial user name to be the email, make sure it's a valid email address
            var identity = await _userIdentityManager.CreateIdentity(info.AccountEmail, user.Id);

            //add the password credential
            await _credentialManager.AddCredential(user.Id, new Credential
            {
                Visibility = CredentialVisibility.Secret,
                Status = Constants.CredentialStatus_Active,
                Uniqueness = Uniqueness.None,
                Name = Constants.CredentialType_Password,
                Data = info.AccountPassword
            });

            //add the contact detail for the email
            var contact = await _contactManager
                .AddContactData(user.Id, new ContactData
                {
                    Data = info.AccountEmail,
                    Channel = Constants.ContactChannel_Email,
                    Status = Constants.ContactStatus_Active
                });

            //create the farmer
            var farmer = await _farmerManager.CreateFarmer(user.Id, new Farmer
            {
                EnterpriseName = info.EnterpriseName,
                Farms = farms
                    .Select(farmName => new Farm {Name = farmName})
                    .ToArray()
            });

            //add the farms to the cooperative
            await farmer.Farms
                .Select(farm => _cooperativeManager.AddFarm(coop.Id, farm.Id))
                .Fold();

            //request account-verification multi-factor authentication
            var token = await _multiFactorAuth.RequestMultiFactorToken(
                user.Id,
                Constants.MultiFactorEventLabels_FarmerAccountVerification,
                contact.Id);

            if (token == null)
                throw new GaiaException(ErrorCodes.GeneralError);

            else
                return new AccountRegistrationResult { Token = token };
        });

        
        public Operation ValidateCooperativeAccountRegistration(MultiFactorAuthenticationInfo authInfo)
        => Operation.Try(async () =>
        {
            await authInfo
                .ThrowIfNull(new GaiaException(ErrorCodes.GeneralError))
                .Validate();

            await _multiFactorAuth.ValidateMultiFactorToken(
                authInfo.UserId, 
                Constants.MultiFactorEventLabels_CooperativeAdminAccountVerification, 
                authInfo.CredentialKey,
                authInfo.CredentialToken);
        });

        public Operation ValidateFarmerAccountRegistration(MultiFactorAuthenticationInfo authInfo)
        => Operation.Try(async () =>
        {
            await authInfo
                .ThrowIfNull(new GaiaException(ErrorCodes.GeneralError))
                .Validate();

            await _multiFactorAuth.ValidateMultiFactorToken(
                authInfo.UserId,
                Constants.MultiFactorEventLabels_FarmerAccountVerification,
                authInfo.CredentialKey,
                authInfo.CredentialToken);
        });
    }
}
