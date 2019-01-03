using System;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{

    public interface IHarvestManager
    {
        Operation<HarvestBatch> CreateHarvestBatch(Guid farmId, HarvestBatch batch);

        /// <summary>
        /// facilitates modification of title and date
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        Operation<HarvestBatch> UpdateHarvestBatch(HarvestBatch batch);

        /// <summary>
        /// Once a batch has been published, non of the data within it can be modified
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        Operation PublishBatch(Guid batchId);


        Operation<ArrayPage<HarvestBatch>> GetHarvestBatches(Guid farmId, ArrayPageRequest request = null);
        Operation<ArrayPage<HarvestBatch>> GetHarvestBatches(ArrayPageRequest request = null);
        #region harvest

        Operation<Harvest> CreateHarvest(Guid batchId, Harvest harvest);

        /// <summary>
        /// Facilitates updating values of a harvest belonging to a draft batch
        /// </summary>
        /// <param name="harvest"></param>
        /// <returns></returns>
        Operation<Harvest> UpdateHarvest(Harvest harvest);


        Operation<ArrayPage<HarvestBatch>> GetHarvests(Guid farmId, Guid bachId, ArrayPageRequest request = null);
        Operation<ArrayPage<HarvestBatch>> GetHarvests(Guid farmId, ArrayPageRequest request = null);
        Operation<ArrayPage<HarvestBatch>> GetHarvests(ArrayPageRequest request = null);
        #endregion
    }
}
