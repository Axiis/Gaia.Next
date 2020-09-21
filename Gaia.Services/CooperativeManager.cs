using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Services.AccessDescriptors;
using Gaia.Services.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class CooperativeManager: ICooperativeManager
    {
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAccessAuthorizer;
        private readonly ICooperativeManagerQueries _queries;


        public CooperativeManager(
            IDataAccessAuthorizer dataAuthorizer,
            ICooperativeManagerQueries queries,
            StoreProvider storeProvider)
        {
            ThrowNullArguments(
                nameof(dataAuthorizer).ObjectPair(dataAuthorizer),
                nameof(queries).ObjectPair(queries),
                nameof(storeProvider).ObjectPair(storeProvider));

            _storeProvider = storeProvider;
            _dataAccessAuthorizer = dataAuthorizer;
            _queries = queries;
        }

        public Operation<Cooperative> CreateCooperative(Cooperative cooperative)
        => Operation.Try(async () =>
        {
            await cooperative
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();
            
            cooperative.Id = Guid.NewGuid();
            cooperative.Admins = null;
            cooperative.Farms = null;
            cooperative.Status = CooperativeStatus.Active;

            var storeCommand = _storeProvider.CommandFor(typeof(Cooperative).FullName);

            return (await storeCommand
                .Add(cooperative))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Cooperative> UpdateCooperative(Cooperative cooperative)
        => Operation.Try(async () =>
        {
            await cooperative
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var persisted = (await _queries
                .GetCooperative(cooperative.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(persisted.Id, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            //copy values
            persisted.Title = cooperative.Title;
            persisted.Address = cooperative.Address;
            persisted.ContactEmail = cooperative.ContactEmail;
            persisted.ContactMobile = cooperative.ContactMobile;
            persisted.ContactName = cooperative.ContactName;

            var storeCommand = _storeProvider.CommandFor(typeof(Cooperative).FullName);
            return (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation UpdateCooperativeStatus(Guid cooperativeId, CooperativeStatus status)
        => Operation.Try(async () =>
        {
            if (cooperativeId == default(Guid))
                throw new GaiaException(PolluxErrorCodes.InvalidArgument);

            var persisted = (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
            

            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(persisted.Id, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            persisted.Status = status;
            var storeCommand = _storeProvider.CommandFor(typeof(Cooperative).FullName);

            (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation AddAdmin(Guid cooperativeId, Guid userId)
        => Operation.Try(async () =>
        {
            //validate argument
            cooperativeId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //validate argument
            userId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var persisted = (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var user = (await _queries
                .GetUser(userId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));


            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(persisted.Id, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            //make sure we are not adding ourselves
            if (admins.Any(admin => admin.User.Id == userId))
                throw new GaiaException(ErrorCodes.DomainLogicError);

            //finally add a new admin object for the cooperative
            var coopAdmin = new CooperativeAdmin
            {
                User = user,
                Cooperative = persisted
            };

            var storeCommand = _storeProvider.CommandFor(typeof(CooperativeAdmin).FullName);

            (await storeCommand
                .Add(coopAdmin))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation RemoveAdmin(Guid cooperativeId, Guid userId)
        => Operation.Try(async () =>
        {
            //validate argument
            cooperativeId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //validate argument
            userId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //make sure the cooperative exists
            (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //make sure the user exists
            (await _queries
                .GetUser(userId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));


            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(cooperativeId, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            //get the admin to be removed
            var coopAdmin = admins
                .FirstOrDefault(admin => admin.User.Id == userId);

            if (coopAdmin == null)
                return; //ignore situations where the admin to be removed does not belong to the list of admins.

            var storeCommand = _storeProvider.CommandFor(typeof(CooperativeAdmin).FullName);

            (await storeCommand
                .Delete(coopAdmin))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation AddFarm(Guid cooperativeId, Guid farmId)
        => Operation.Try(async () =>
        {
            //validate argument
            cooperativeId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //validate argument
            farmId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var cooperative = (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));


            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(cooperativeId, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            //filter out duplicates
            (await _queries
                .GetCooperativeFarm(cooperativeId, farmId))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //add the farm
            var coopFarm = new CooperativeFarm
            {
                Farm = farm,
                Cooperative =  cooperative
            };

            var storeCommand = _storeProvider.CommandFor(typeof(CooperativeFarm).FullName);
            (await storeCommand
                .Add(coopFarm))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation RemoveFarm(Guid cooperativeId, Guid farmId)
        => Operation.Try(async () =>
        {
            //validate argument
            cooperativeId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //validate argument
            farmId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            //make sure the cooperative exists
            (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //make sure the farm exists
            (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));


            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(cooperativeId, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            //get the Farm map to be removed
            var coopFarm = await _queries.GetCooperativeFarm(cooperativeId, farmId);

            if (coopFarm == null)
                return; //ignore situations where the admin to be removed does not belong to the list of admins.

            var storeCommand = _storeProvider.CommandFor(typeof(CooperativeFarm).FullName);

            (await storeCommand
                .Delete(coopFarm))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Cooperative> GetCooperative(Guid cooperativeId)
        => Operation.Try(async () =>
        {
            //validate parameter
            cooperativeId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));
            

            var pageRequest = ArrayPageRequest.CreateNormalizedRequest();
            var admins = await pageRequest.GetAll(r => _queries.GetAdmins(cooperativeId, r));
            admins.ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //Ensure that the right principal has access to this data
            await _dataAccessAuthorizer.AuthorizeAccess(new CoopAdminOwnedData(admins));

            return (await _queries
                .GetCooperative(cooperativeId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Cooperative>> GetAdminCooperatives(Guid userId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            userId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            return (await _queries
                .GetAdminCooperatives(userId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Cooperative>> GetFarmerCooperatives(Guid farmerId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            farmerId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            return (await _queries
                .GetFarmerCooperatives(farmerId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Cooperative>> GetAllCooperatives(ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            return (await _queries
                .GetAllCooperatives(request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Farmer>> GetRegisteredFarmers(Guid cooperativeId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            cooperativeId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            return (await _queries
                .GetRegisteredFarmers(cooperativeId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Farm>> GetRegisteredFarms(Guid cooperativeId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            cooperativeId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            return (await _queries
                .GetRegisteredFarms(cooperativeId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });
    }
}
