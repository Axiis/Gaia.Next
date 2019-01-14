using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Pollux.Authentication.Models;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities.Authentication
{

    public class Credential : BaseEntity<Guid>
    {
        /// <summary>
        /// Indicates that this credential's "data" should be unique across the system, given the "name" value
        /// </summary>
        public Uniqueness Uniqueness { get; set; }
        public CredentialVisibility Visibility { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// Credential Name (password, username, etc)
        /// </summary>
        public string Name { get; set; }
        public string Data { get; set; }
        public DateTimeOffset? ExpiresOn { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class CredentialTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public CredentialTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Credential);
            this.ModelType = typeof(Axis.Pollux.Authentication.Models.Credential);

            this.CreateEntity = model => new Credential();
            this.CreateModel = entity => new Axis.Pollux.Authentication.Models.Credential();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Authentication.Models.Credential) m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Credential) e, 
                    command, 
                    context, 
                    _resolver);

                entity.Data = model.Data;
                entity.ExpiresOn = model.ExpiresOn;
                entity.Name = model.Name;
                entity.Status = model.Status;
                entity.Uniqueness = model.Uniqueness;
                entity.Visibility = model.Visibility;

                entity.Owner = (User) context.Transformer.ToEntity(model.Owner, command, context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Credential) e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Authentication.Models.Credential) m, 
                    command, 
                    context,
                    _resolver);

                model.Data = entity.Data;
                model.ExpiresOn = entity.ExpiresOn;
                model.Name = entity.Name;
                model.Status = entity.Status;
                model.Uniqueness = entity.Uniqueness;
                model.Visibility = entity.Visibility;

                model.Owner = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.Owner, 
                    command, 
                    context);
            };
        }
    }
}
