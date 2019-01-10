using System;
using System.Collections.Generic;
using System.Text;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Luna.Operation;
using Axis.Pollux.Common.Models;
using Axis.Pollux.Identity.Contracts;
using Axis.Proteus.Ioc;
using Gaia.Core.Exceptions;
using Gaia.Data.EFCore.Entities;

namespace Gaia.Data.EFCore
{
    public static class Extensions
    {
        public static TEntity TransformBase<TKey, TEntity>(
            this IBaseModel<TKey> model,
            TEntity entity,
            TransformCommand command,
            ModelTransformationContext context,
            IServiceResolver resolver)
        where TEntity: BaseEntity<TKey>
        {
            var userContext = resolver.Resolve<IUserContext>();
            var currentUserId = userContext
                .CurrentUserId()
                .Catch(ex => Guid.Empty)
                .Resolve(); //should return null instead of throwing exceptions for missing UserContexts

            entity.Id = model.Id;

            entity.CreatedBy = 
                command == TransformCommand.Add
                ? currentUserId.ThrowIf(default(Guid), new GaiaException(ErrorCodes.DomainLogicError))
                : model.CreatedBy;
            entity.CreatedOn =
                command == TransformCommand.Add
                ? DateTimeOffset.Now
                : model.CreatedOn;

            entity.ModifiedBy =
                command == TransformCommand.Update
                ? currentUserId.ThrowIf(default(Guid), new GaiaException(ErrorCodes.DomainLogicError))
                : model.CreatedBy;
            entity.ModifiedOn =
                command == TransformCommand.Update
                ? DateTimeOffset.Now
                : model.ModifiedOn;

            return entity;
        }


        public static TModel TransformBase<TKey, TModel>(
            this BaseEntity<TKey> entity,
            TModel model,
            TransformCommand command,
            ModelTransformationContext context, 
            IServiceResolver resolver)
        where TModel: IBaseModel<TKey>
        {
            model.Id = entity.Id;
            model.CreatedBy = entity.CreatedBy;
            model.CreatedOn = entity.CreatedOn;
            model.ModifiedBy = entity.ModifiedBy;
            model.ModifiedOn = entity.ModifiedOn;

            return model;
        }
    }
}
