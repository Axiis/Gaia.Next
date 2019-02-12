using System;
using System.Collections.Generic;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Operation;
using Axis.Pollux.Authorization.Abac.Models;
using Axis.Sigma.Policy;
using Gaia.Core.Exceptions;
using Gaia.Services.Queries;
using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Services
{
    public class PolicyWriter : IPolicyWriter
    {
        private readonly StoreProvider _provider;
        private readonly IPolicyWriterQuery _query;

        public PolicyWriter(StoreProvider provider, IPolicyWriterQuery query)
        {
            ThrowNullArguments(
                () => provider,
                () => query);

            _provider = provider;
            _query = query;
        }

        public Operation Persist(IEnumerable<Policy> policies)
        => Operation.Try(async () =>
        {
            if (policies == null)
                return;
            //else
            await policies
                .Cast<PolluxPolicy>()
                .Where(policy => policy != null)
                .Select(Persist)
                .Fold();
        });

        private Operation Persist(PolluxPolicy policy)
        => Operation.Try(async () =>
        {
            var persisted = policy.Id != default(Guid)
                ? (await _query.GetPolicyById(policy.Id)).ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreQueryResult))
                : new PolluxPolicy();

            persisted.Parent = policy.Parent;
            persisted.Code = policy.Code;
            persisted.CombinationClause = policy.CombinationClause;
            persisted.GovernedResources = policy.GovernedResources;
            persisted.Title = policy.Title;

            var command = _provider.CommandFor(typeof(PolluxPolicy).FullName);

            var op = policy.Id == default(Guid)
                ? command.Add(persisted)
                : command.Update(persisted);

            (await op).ThrowIfNull(new GaiaException(ErrorCodes.InvalidStoreCommandResult));
        });
    }
}
