using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Axis.Pollux.Authentication.Models;
using Axis.Pollux.Authentication.Services.Queries;
using Axis.Pollux.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Data.EFCore.Queries.Pollux.Authentication
{
    public class AuthenticatorQueries : IAuthenticatorQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public AuthenticatorQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<bool> ContainsPublicCredentials(Guid userId, string credentialName, string credentialData)
        {
            return await _query
                .Query<Entities.Authentication.Credential>()
                .Where(c => userId.CompareTo(c.OwnerId) == 0)
                .Where(c => c.Name == credentialName)
                .Where(c => c.Data == credentialData)
                .Where(c => c.Visibility == CredentialVisibility.Public)
                .AnyAsync();
        }

        public async Task<bool> ContainsPublicCredentials(string credentialName, string credentialData)
        {
            return await _query
                .Query<Entities.Authentication.Credential>()
                .Where(c => c.Name == credentialName)
                .Where(c => c.Data == credentialData)
                .Where(c => c.Visibility == CredentialVisibility.Public)
                .AnyAsync();
        }

        public async Task<ArrayPage<Credential>> GetActiveSystemUniqueCredentials(string credentialName, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Authentication.Credential>(d => d.Owner)
                .Where(d => credentialName == d.Name);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Credential>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Authentication.Credential, Credential>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<Credential>> GetActiveUserCredentials(Guid ownerId, string credentialName, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Authentication.Credential>(d => d.Owner)
                .Where(d => credentialName == d.Name);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int)(request.PageIndex * request.PageSize))
                .Take((int)request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<Credential>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Authentication.Credential, Credential>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<Credential> GetCredentialById(Guid credentialId)
        {
            var data = await _query
                .Query<Entities.Authentication.Credential>(d => d.Owner)
                .Where(d => credentialId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<Credential>(data, TransformCommand.Query);
        }
    }
}
