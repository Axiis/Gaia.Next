using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class UserIdentity : BaseEntity<Guid>
    {
        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
        
        public string UserName { get; set; }
    }
    

    public class UserIdentityTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public UserIdentityTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(UserIdentity);
            this.ModelType = typeof(Core.Models.UserIdentity);

            this.CreateEntity = model => new UserIdentity();
            this.CreateModel = entity => new Core.Models.UserIdentity();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Core.Models.UserIdentity)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (UserIdentity)e,
                    command,
                    context,
                    _resolver);

                entity.UserName = model.UserName;
                entity.Owner = (User) context.Transformer.ToEntity(
                    model.Owner,
                    command,
                    context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (UserIdentity)e;
                var model = entity.TransformBase(
                    (Core.Models.UserIdentity)m,
                    command,
                    context,
                    _resolver);

                model.Owner = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.Owner,
                    command,
                    context);
            };
        }
    }
}
