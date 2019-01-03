using System;
using System.Collections.Generic;
using System.Text;
using Axis.Jupiter;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts;
using Gaia.Core.CustomDataAccessAuth;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Services.Queries;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class FarmerManager: IFarmerManager
    {
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAuth;
        private readonly IFarmerManagerQueries _queries;


        public FarmerManager(
            IDataAccessAuthorizer dataAuthorizer,
            IFarmerManagerQueries queries,
            StoreProvider storeProvider)
        {
            ThrowNullArguments(
                () => dataAuthorizer,
                () => queries,
                () => storeProvider);

            _storeProvider = storeProvider;
            _dataAuth = dataAuthorizer;
            _queries = queries;
        }


        public Operation<Farmer> CreateFarmer(Guid userId, Farmer farmer)
        => Operation.Try(async () =>
        {
            //verify the parameters
            userId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            await farmer
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            //data access authorization
            await _dataAuth.AuthorizeAccess(typeof(Farmer).FullName);

            var user = (await _queries
                .GetUser(userId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //farmer.Id = Guid.NewGuid(); //<-- this is redundant because the StoreTransformer always adds new Guids for objects being added to the store.
            farmer.Farms = new Farm[0];
            farmer.Status = FarmerStatus.Active;
            farmer.User = user;

            var storeCommand = _storeProvider.CommandFor(typeof(Farmer).FullName);
            return (await storeCommand
                .Add(farmer))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Farmer> UpdateFarmer(Farmer farmer)
        => Operation.Try(async () =>
        {
            //verify the 
            await farmer
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            //data access authorization
            await _dataAuth.AuthorizeAccess(
                typeof(Farmer).FullName,
                farmer.User.Id,
                farmer.Id.ToString());

            var persisted = (await _queries
                .GetFarmer(farmer.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //copy values
            persisted.EnterpriseName = farmer.EnterpriseName;

            var storeCommand = _storeProvider.CommandFor(typeof(Farmer).FullName);
            return (await storeCommand
                .Update(farmer))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation UpdateFarmerStatus(Guid farmerId, FarmerStatus status)
        => Operation.Try(async () =>
        {
            //verify the 
            farmerId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farmer = (await _queries
                .GetFarmer(farmerId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeAccess(
                typeof(Farmer).FullName,
                farmer.User.Id,
                farmer.Id.ToString());

            var persisted = (await _queries
                .GetFarmer(farmer.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //copy values
            persisted.Status = status;

            var storeCommand = _storeProvider.CommandFor(typeof(Farmer).FullName);
            (await storeCommand
                .Update(farmer))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Farm> AddFarm(Guid farmerId, Farm farm)
        => Operation.Try(async () =>
        {
            //validate parameters
            farmerId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            await farm
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var farmer = (await _queries
                .GetFarmer(farmerId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess {Farmer = farmer});

            //perform the edit
            //farmer.Id = Guid.Empty; //<-- unnecessary since the Transformer for the data store will add a new Guid
            farm.Owner = farmer;

            var storeCommand = _storeProvider.CommandFor(typeof(Farm).FullName);
            return (await storeCommand
                .Add(farm))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Farm> DeleteFarm(Guid farmerId, Guid farmId)
        => Operation.Try(async () =>
        {
            //validate parameters
            farmerId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            farmId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farmer = (await _queries
                .GetFarmer(farmerId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farmer,
                Farm = farm
            });

            //perform the delete
            var storeCommand = _storeProvider.CommandFor(typeof(Farm).FullName);
            return (await storeCommand
                .Delete(farm))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<ArrayPage<Farmer>> GetFarmers(ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //data access authorization
            await _dataAuth.AuthorizeAccess(typeof(Farmer).FullName);

            return (await _queries
                .GetAllFarmers(request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });
    }
}
