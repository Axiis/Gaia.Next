using System;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class User: BaseEntity<Guid>
    {
        public int Status { get; set; }
    }

    public class UserTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public UserTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(User);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.User);

            this.CreateEntity = model => new User();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.User();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.User)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (User)e,
                    command,
                    context,
                    _resolver);

                entity.Status = model.Status;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (User)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.User)m,
                    command,
                    context,
                    _resolver);

                model.Status = entity.Status; 
            };
        }
    }
}
