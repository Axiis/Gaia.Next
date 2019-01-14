using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class Farmer : BaseEntity<Guid>
    {
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }

        public string EnterpriseName { get; set; }
        public Core.Models.FarmerStatus Status { get; set; }

        public virtual ICollection<Farm> Farms { get; set; } = new HashSet<Farm>();
    }

    public class FarmerTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public FarmerTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Farmer);
            this.ModelType = typeof(Core.Models.Farmer);

            this.CreateEntity = model => new Farmer();
            this.CreateModel = entity => new Core.Models.Farmer();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.Farmer)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Farmer)e,
                    command,
                    context,
                    _resolver);

                entity.User = (User) context.Transformer.ToEntity(
                    model.User,
                    command,
                    context);
                entity.UserId = model.User?.Id ?? default(Guid);
                entity.EnterpriseName = model.EnterpriseName;
                entity.Status = model.Status;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Farmer)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.Farmer)m,
                    command,
                    context,
                    _resolver);

                model.User = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.User,
                    command,
                    context);
                model.EnterpriseName = entity.EnterpriseName;
                model.Status = entity.Status;
                model.Farms = entity.Farms?
                    .Select(farm => context.Transformer.ToModel<Core.Models.Farm>(
                        farm,
                        command,
                        context))
                    .ToArray() ?? new Core.Models.Farm[0];
            };
        }
    }
}
