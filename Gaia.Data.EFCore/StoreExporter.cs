using Axis.Jupiter;
using Gaia.Data.EFCore.Contexts;

namespace Gaia.Data.EFCore
{
    public static class StoreExporter
    {
        /// <summary>
        /// Return the store maps exported from this assembly
        /// </summary>
        /// <returns></returns>
        public static StoreMap.Entry[] ExportStoreEntries() => new[]
        {
            new StoreMap.Entry
            { 
                StoreName = "Gaia.DomainStore",
                StoreCommandType = typeof(DomainStoreCommand),
                StoreQueryType = typeof(DomainStoreQuery)
            }
        };
    }
}
