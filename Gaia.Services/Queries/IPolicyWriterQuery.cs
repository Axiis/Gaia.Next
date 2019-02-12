using Axis.Pollux.Authorization.Abac.Models;
using System;
using System.Threading.Tasks;

namespace Gaia.Services.Queries
{
    public interface IPolicyWriterQuery
    {
        Task<PolluxPolicy> GetPolicyById(Guid policyId);
    }
}
