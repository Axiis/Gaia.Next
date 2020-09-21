using System;
using Axis.Jupiter;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Core.Utils;
using Gaia.Services.Queries;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class FarmManager: IFarmManager
    {
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAuth;
        private readonly IFarmManagerQueries _queries;


        public FarmManager(
            IDataAccessAuthorizer dataAuthorizer,
            IFarmManagerQueries queries,
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


        public Operation<Farm> CreateFarm(Guid farmerId, Farm farm)
        => Operation.Try(async () =>
        {
            //verify the parameters
            farmerId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            await farm
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            //data access authorization
            await _dataAuth.AuthorizeAccess(typeof(Farm).FullName);

            var farmer = (await _queries
                .GetFarmer(farmerId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //farm.Id = Guid.NewGuid(); //<-- this is redundant because the StoreTransformer always adds new Guids for objects being added to the store.
            farm.Cooperatives = new Cooperative[0];
            farm.Products = new ProductCategory[0];
            farm.Harvests = new HarvestBatch[0];
            farm.Area = new GeoPosition[0];

            var storeCommand = _storeProvider.CommandFor(typeof(Farm).FullName);
            return (await storeCommand
                .Add(farm))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Farm> UpdateFarm(Farm farm)
        => Operation.Try(async () =>
        {
            //verify the 
            await farm
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            //data access authorization
            await _dataAuth.AuthorizeAccess(
                typeof(Farm).FullName,
                farm.Id.ToString());

            var persisted = (await _queries
                .GetFarm(farm.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //copy values
            persisted.Area = farm.Area;
            persisted.Description = farm.Description;
            persisted.LocationDescription = farm.LocationDescription;
            persisted.Name = farm.Name;

            var storeCommand = _storeProvider.CommandFor(typeof(Farmer).FullName);
            return (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Farm> UpdateFarmGeoAreaData(Guid farmId, GeoPosition[] updatedGeoArea)
        => Operation.Try(async () =>
        {
            //verify the 
            farmId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            //do whatever calculation is necessary to validate the 'updatedGeoArea'

            //data access authorization
            await _dataAuth.AuthorizeAccess(
                typeof(Farm).FullName,
                farmId.ToString());

            var persisted = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //copy values
            persisted.Area = updatedGeoArea ?? new GeoPosition[0];

            var storeCommand = _storeProvider.CommandFor(typeof(Farmer).FullName);
            return (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation AddProductCategory(Guid farmId, Guid produceId)
        => Operation.Try(async () =>
        {
            //validate parameters
            farmId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            produceId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var produce = (await _queries
                .GetFarmProduce(produceId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            //filter out duplicates
            (await _queries
                .GetProductCategory(farmId, produceId))
                .ThrowIfNotNull(new GaiaException(ErrorCodes.DomainLogicError));

            //perform the edit
            var category = new ProductCategory
            {
                FarmProduce = produce,
                Farm = farm
            };

            var storeCommand = _storeProvider.CommandFor(typeof(ProductCategory).FullName);
            (await storeCommand
                .Add(category))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation RemoveProductCategory(Guid farmId, Guid productId)
        => Operation.Try(async () =>
        {
            //validate parameters
            farmId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            productId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            (await _queries
                .GetFarmProduce(productId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //check authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            var productCategory = (await _queries
                .GetProductCategory(farmId, productId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //perform the delete
            var storeCommand = _storeProvider.CommandFor(typeof(ProductCategory).FullName);
            (await storeCommand
                .Delete(productCategory))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<ArrayPage<Farm>> GetFarms(ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            await _dataAuth.AuthorizeAccess(typeof(Farm).FullName);

            return (await _queries
                .GetFarms(request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });
    }
}