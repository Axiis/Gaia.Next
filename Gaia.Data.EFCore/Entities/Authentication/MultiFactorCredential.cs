using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Axis.Jupiter;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities.Authentication
{
    public class MultiFactorCredential : BaseEntity<Guid>
    {
        [ForeignKey(nameof(TargetUserId))]
        public virtual User TargetUser { get; set; }
        public Guid TargetUserId { get; set; }

        public string CredentialKey { get; set; }
        public string CredentialToken { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// A string representing the entity - usually a contract operation call, that the multi-factor authentication is being employed to guard"
        /// </summary>
        public string EventLabel { get; set; }
    }

    public class MultiFactorCredentialTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public MultiFactorCredentialTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(MultiFactorCredential);
            this.ModelType = typeof(Axis.Pollux.Authentication.Models.MultiFactorCredential);

            this.CreateEntity = model => new MultiFactorCredential();
            this.CreateModel = entity => new Axis.Pollux.Authentication.Models.MultiFactorCredential();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Authentication.Models.MultiFactorCredential)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (MultiFactorCredential) e,
                    command,
                    context,
                    _resolver);

                entity.CredentialKey = model.CredentialKey;
                entity.CredentialToken = model.CredentialToken;
                entity.EventLabel = model.EventLabel;
                entity.ExpiresOn = model.ExpiresOn;
                entity.IsAuthenticated = model.IsAuthenticated;

                entity.TargetUser = (User) context.Transformer.ToEntity(model.TargetUser, command, context);
                entity.TargetUserId = entity.TargetUser?.Id ?? Guid.Empty;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (MultiFactorCredential)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Authentication.Models.MultiFactorCredential)m,
                    command,
                    context,
                    _resolver);

                model.CredentialKey = entity.CredentialKey;
                model.CredentialToken = entity.CredentialToken;
                model.EventLabel = entity.EventLabel;
                model.ExpiresOn = entity.ExpiresOn;
                model.IsAuthenticated = entity.IsAuthenticated;

                model.TargetUser = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(entity.TargetUser, command, context);
            };
        }
    }
}
