using System;
using Axis.Jupiter;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Contracts;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Contracts;
using Gaia.Core.CustomDataAccessAuth;
using Gaia.Core.Exceptions;
using Gaia.Core.Models;
using Gaia.Core.Utils;
using Gaia.Services.Queries;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    using PolluxErrorCodes = Axis.Pollux.Common.Exceptions.ErrorCodes;

    public class HarvestManager: IHarvestManager
    {
        private readonly StoreProvider _storeProvider;
        private readonly IDataAccessAuthorizer _dataAuth;
        private readonly IHarvestManagerQueries _queries;


        public HarvestManager(
            IDataAccessAuthorizer dataAuthorizer,
            IHarvestManagerQueries queries,
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


        public Operation<HarvestBatch> CreateHarvestBatch(Guid farmId, HarvestBatch batch)
        => Operation.Try(async () =>
        {
            //verify the parameters
            farmId
                .ThrowIf(default(Guid), new GaiaException(PolluxErrorCodes.InvalidArgument));

            await batch
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization: make sure the current user is the farmer that owns the farm to which the batch is being added
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            //batch.Id = Guid.NewGuid(); //<-- this is redundant because the StoreTransformer always adds new Guids for objects being added to the store.
            batch.Harvests = new Harvest[0];
            batch.Farm = farm;
            batch.Status = HarvestBatchStatus.Draft;

            var storeCommand = _storeProvider.CommandFor(typeof(HarvestBatch).FullName);
            return (await storeCommand
                .Add(batch))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<HarvestBatch> UpdateHarvestBatch(HarvestBatch batch)
        => Operation.Try(async () =>
        {
            //verify the 
            await batch
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var persisted = (await _queries
                    .GetBatch(batch.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farm = (await _queries
                .GetFarm(persisted.Farm.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            //if the batch is published, do not allow modification
            persisted.ThrowIf(
                BatchIsPublished, 
                new GaiaException(ErrorCodes.DomainLogicError));

            //copy values
            persisted.BatchDate = batch.BatchDate;
            persisted.BatchTitle = batch.BatchTitle;

            var storeCommand = _storeProvider.CommandFor(typeof(HarvestBatch).FullName);
            return (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation PublishBatch(Guid batchId)
        => Operation.Try(async () =>
        {
            //validate parameter
            batchId.ThrowIf(
                default(Guid), 
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var batch = (await _queries
                .GetBatch(batchId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farm = (await _queries
                .GetFarm(batch.Farm.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            if (batch.Status == HarvestBatchStatus.Published)
                return;

            //else
            batch.Status = HarvestBatchStatus.Published;

            var storeCommand = _storeProvider.CommandFor(typeof(HarvestBatch).FullName);
            (await storeCommand
                .Update(batch))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<ArrayPage<HarvestBatch>> GetHarvestBatches(Guid farmId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            farmId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            return (await _queries
                .GetHarvestBatches(farmId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<HarvestBatch>> GetHarvestBatches(ArrayPageRequest request = null)
         => Operation.Try(async () =>
         {
             //data access authorization
             await _dataAuth.AuthorizeAccess(typeof(HarvestBatch).FullName);

             return (await _queries
                 .GetHarvestBatches(request))
                 .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
         });


        public Operation<Harvest> CreateHarvest(Guid batchId, Harvest harvest)
        => Operation.Try(async () =>
        {
            //verify the parameters
            batchId.ThrowIf(
                default(Guid), 
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            await harvest
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var batch = (await _queries
                .GetBatch(batchId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farmer = (await _queries
                .GetFarmer(batch.Farm.Owner.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization: make sure the current user is the farmer that owns the farm to which the batch is being added
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farmer,
                Farm = batch.Farm
            });

            //cannot add a harvest to an already published batch
            if(batch.Status == HarvestBatchStatus.Published)
                throw new GaiaException(ErrorCodes.DomainLogicError);

            //harvest.Id = Guid.NewGuid(); //<-- this is redundant because the StoreTransformer always adds new Guids for objects being added to the store.
            harvest.Batch = batch;

            var storeCommand = _storeProvider.CommandFor(typeof(Harvest).FullName);
            return (await storeCommand
                .Add(harvest))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<Harvest> UpdateHarvest(Harvest harvest)
        => Operation.Try(async () =>
        {
            //verify the 
            await harvest
                .ThrowIfNull(new GaiaException(PolluxErrorCodes.InvalidArgument))
                .Validate();

            var persisted = (await _queries
                .GetHarvest(harvest.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            var farm = (await _queries
                .GetFarm(persisted.Batch.Farm.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            //cannot update harvest of a published batch
            if (persisted.Batch.Status == HarvestBatchStatus.Published)
                throw new GaiaException(ErrorCodes.DomainLogicError);

            //make sure the new harvest product exists
            var produce = (await _queries
                .GetFarmProduce(harvest.Produce.Id))
                .ThrowIfNull(new GaiaException(ErrorCodes.DomainLogicError));

            //copy values
            persisted.Produce = produce;
            persisted.Unit = harvest.Unit;
            persisted.UnitAmount = harvest.UnitAmount;

            var storeCommand = _storeProvider.CommandFor(typeof(HarvestBatch).FullName);
            return (await storeCommand
                .Update(persisted))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });

        public Operation<ArrayPage<Harvest>> GetHarvests(Guid farmId, Guid batchId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            farmId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            batchId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            return (await _queries
                .GetHarvests(farmId, batchId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Harvest>> GetHarvests(Guid farmId, ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //validate parameter
            farmId.ThrowIf(
                default(Guid),
                new GaiaException(PolluxErrorCodes.InvalidArgument));

            var farm = (await _queries
                .GetFarm(farmId))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));

            //data access authorization
            await _dataAuth.AuthorizeCustomAccess(new FarmerDataAccess
            {
                Farmer = farm.Owner,
                Farm = farm
            });

            return (await _queries
                .GetHarvests(farmId, request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });

        public Operation<ArrayPage<Harvest>> GetHarvests(ArrayPageRequest request = null)
        => Operation.Try(async () =>
        {
            //data access authorization
            await _dataAuth.AuthorizeAccess(typeof(Harvest).FullName);

            return (await _queries
                .GetHarvests(request))
                .ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult));
        });


        private static bool BatchIsPublished(HarvestBatch batch) => batch?.Status == HarvestBatchStatus.Published;
    }
}
