using System;
using System.Linq;
using Gaia.Core.Models;

namespace Gaia.Services.AccessDescriptors
{
    /// <summary>
    /// Custom data authorization param for evaluating if the current user is one of the admins
    /// in the list provided, or, if the current user has access to all Cooperative data
    /// </summary>
    public class CoopAdminOwnedData: PropertyCollectionDescriptor
    {
        public CoopAdminOwnedData(CooperativeAdmin[] admins)
        {
            AdminUserIds = admins?
                .Select(admin => admin.User.Id)
                .ToArray();
        }

        public Guid[] AdminUserIds { get; }
    }
}
