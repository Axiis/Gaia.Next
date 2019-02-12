using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Axis.Luna.Extensions;
using Axis.Pollux.Common.Utils;
using Axis.Pollux.Identity.Models;
using Axis.Pollux.Identity.Services.Queries;
using Microsoft.EntityFrameworkCore;
using static Axis.Luna.Extensions.ExceptionExtension;

namespace Gaia.Data.EFCore.Queries.Pollux
{
    public class IdentityUserQueries : IUserQueries
    {
        private readonly IEFStoreQuery _query;
        private readonly ModelTransformer _transformer;

        public IdentityUserQueries(IEFStoreQuery query, ModelTransformer transformer)
        {
            ThrowNullArguments(
                () => query,
                () => transformer);

            _query = query;
            _transformer = transformer;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var userEntity = await _query
                .Query<Entities.Identity.User>()
                .Where(user => user.Id.CompareTo(userId) == 0)
                .FirstOrDefaultAsync();

            return userEntity == null
                ? null
                : _transformer.ToModel<User>(userEntity, TransformCommand.Query);
        }

        public async Task<long> UserCount()
        {
            return await _query
                .Query<Entities.Identity.User>()
                .CountAsync();
        }

        public async Task<ArrayPage<AddressData>> GetUserAddressData(Guid userId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Identity.AddressData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int) (request.PageIndex * request.PageSize))
                .Take((int) request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<AddressData>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Identity.AddressData, AddressData>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<ContactData>> GetUserContactData(Guid userId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Identity.ContactData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int) (request.PageIndex * request.PageSize))
                .Take((int) request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<ContactData>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Identity.ContactData, ContactData>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<NameData>> GetUserNameData(Guid userId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Identity.NameData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int) (request.PageIndex * request.PageSize))
                .Take((int) request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<NameData>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Identity.NameData, NameData>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<ArrayPage<UserData>> GetUserData(Guid userId, ArrayPageRequest request = null)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Identity.UserData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int) (request.PageIndex * request.PageSize))
                .Take((int) request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<UserData>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Identity.UserData, UserData>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        public async Task<BioData> GetUserBioData(Guid userId)
        {
            var data = await _query
                .Query<Entities.Identity.BioData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<BioData>(data, TransformCommand.Query);
        }

        public async Task<BioData> GetBioDataById(Guid bioDataId)
        {
            var data = await _query
                .Query<Entities.Identity.BioData>(d => d.Owner)
                .Where(d => bioDataId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<BioData>(data, TransformCommand.Query);
        }

        public async Task<NameData> GetNameDataById(Guid nameDataId)
        {
            var data = await _query
                .Query<Entities.Identity.NameData>(d => d.Owner)
                .Where(d => nameDataId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<NameData>(data, TransformCommand.Query);
        }

        public async Task<AddressData> GetAddressDataById(Guid addressDataId)
        {
            var data = await _query
                .Query<Entities.Identity.AddressData>(d => d.Owner)
                .Where(d => addressDataId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<AddressData>(data, TransformCommand.Query);
        }

        public async Task<ContactData> GetContactDataById(Guid contactDataId)
        {
            var data = await _query
                .Query<Entities.Identity.ContactData>(d => d.Owner)
                .Where(d => contactDataId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<ContactData>(data, TransformCommand.Query);
        }

        public async Task<UserData> GetUserDataById(Guid userDataId)
        {
            var data = await _query
                .Query<Entities.Identity.UserData>(d => d.Owner)
                .Where(d => userDataId.CompareTo(d.Id) == 0)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<UserData>(data, TransformCommand.Query);
        }

        public async Task<ContactData> GetPrimaryUserContactData(Guid userId, string communicationChannel)
        {
            var data = await _query
                .Query<Entities.Identity.ContactData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0)
                .Where(d => d.IsPrimary)
                .Where(d => d.Channel == communicationChannel)
                .FirstOrDefaultAsync();

            return _transformer.ToModel<ContactData>(data, TransformCommand.Query);
        }

        public async Task<ArrayPage<ContactData>> GetUserContactData(
            Guid userId,
            string[] communicationChannels,
            string[] tags,
            ArrayPageRequest request)
        {
            request = request?.Normalize() ?? ArrayPageRequest.CreateNormalizedRequest();
            var dataQuery = _query
                .Query<Entities.Identity.ContactData>(d => d.Owner)
                .Where(d => userId.CompareTo(d.OwnerId) == 0)
                .Where(d => d.IsPrimary)
                .Where(d => communicationChannels.Contains(d.Channel));

            var tagPredicate = TagPatterns(tags);
            if (tagPredicate != null)
                dataQuery = dataQuery.Where(tagPredicate);

            var longCount = await dataQuery.LongCountAsync();

            var filtered = await dataQuery
                .OrderBy(c => c.CreatedOn)
                .Skip((int) (request.PageIndex * request.PageSize))
                .Take((int) request.PageSize)
                .ToArrayAsync();

            return new ArrayPage<ContactData>
            {
                TotalLength = longCount,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value,
                Page = filtered.TransformQuery<Entities.Identity.ContactData, ContactData>(
                    _transformer,
                    TransformCommand.Query)
            };
        }

        private static Expression<Func<Entities.Identity.ContactData, bool>> TagPatterns(string[] tags)
        {
            if (tags == null || tags.Length == 0) return null;

            var param = Expression.Parameter(typeof(Entities.Identity.ContactData), "data");
            var tagsProp =
                typeof(Entities.Identity.ContactData).GetProperty(nameof(Entities.Identity.ContactData.Tags));
            var method = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like));

            Expression predicate = null;
            tags.Select(tag => Expression.Call(
                    null,
                    method,
                    Expression.MakeMemberAccess(param, tagsProp),
                    Expression.Constant($"%{tag}%")))
                .ForAll(exp =>
                {
                    predicate = predicate == null
                        ? (Expression) exp
                        : Expression.OrElse(predicate, exp);
                });

            return Expression.Lambda<Func<Entities.Identity.ContactData, bool>>(predicate, param);
        }
    }
}
