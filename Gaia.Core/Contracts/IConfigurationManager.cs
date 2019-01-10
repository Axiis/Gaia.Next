using System;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Utils;
using Gaia.Core.Models;

namespace Gaia.Core.Contracts
{
    public interface IConfigurationManager
    {
        Operation<AppConfiguration> GetAppConfiguration(string name, string scope = null);
        Operation<ArrayPage<AppConfiguration>> GetScopedConfigurations(string scope);
        Operation<ArrayPage<AppConfiguration>> GetAllConfigurations();

        Operation<AppConfiguration> AddConfiguration(AppConfiguration configuration, AppConfiguration parent = null);
        Operation<AppConfiguration> UpdateConfiguration(AppConfiguration configuration);
        Operation<AppConfiguration> DeleteConfiguration(Guid configId);
    }
}
