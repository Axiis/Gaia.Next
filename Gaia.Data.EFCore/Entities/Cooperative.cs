using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;

namespace Gaia.Data.EFCore.Entities
{
    public class Cooperative : BaseEntity<Guid>
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }

        public virtual ICollection<CooperativeAdmin> Admins { get; set; } = new HashSet<CooperativeAdmin>();
        public virtual ICollection<Farm> Farms { get; set; } = new HashSet<Farm>();

        public Core.Models.CooperativeStatus Status { get; set; }
    }

    public class CooperativeAdmin : BaseEntity<Guid>
    {
        [ForeignKey(nameof(CooperativeId))]
        public virtual Cooperative Cooperative { get; set; }
        public Guid CooperativeId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }

    public class CooperativeFarm : BaseEntity<Guid>
    {
        [ForeignKey(nameof(CooperativeId))]
        public virtual Cooperative Cooperative { get; set; }
        public Guid CooperativeId { get; set; }

        [ForeignKey(nameof(FarmId))]
        public Farm Farm { get; set; }
        public Guid FarmId { get; set; }
    }


    public class CooperativeTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public CooperativeTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Cooperative);
            this.ModelType = typeof(Core.Models.Cooperative);

            this.CreateEntity = model => new Cooperative();
            this.CreateModel = entity => new Core.Models.Cooperative();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.Cooperative)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Cooperative)e,
                    command,
                    context,
                    _resolver);

                entity.Address = model.Address;
                entity.ContactEmail = model.ContactEmail;
                entity.ContactMobile = model.ContactMobile;
                entity.ContactName = model.ContactName;
                entity.Status = model.Status;
                entity.Title = model.Title;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Cooperative)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.Cooperative)m,
                    command,
                    context,
                    _resolver);

                model.Address = entity.Address;
                model.ContactEmail = entity.ContactEmail;
                model.ContactMobile = entity.ContactMobile;
                model.ContactName = entity.ContactName;
                model.Status = entity.Status;
                model.Title = entity.Title;

                model.Admins = entity.Admins
                    ?.Select(admin => context.Transformer.ToModel<Core.Models.CooperativeAdmin>(
                        admin,
                        command,
                        context))
                    .ToArray();
                model.Farms = entity.Farms
                    ?.Select(farm => context.Transformer.ToModel<Core.Models.CooperativeFarm>(
                        farm,
                        command,
                        context))
                    .ToArray();
            };
        }
    }

    public class CooperativeAdminTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public CooperativeAdminTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(CooperativeAdmin);
            this.ModelType = typeof(Core.Models.CooperativeAdmin);

            this.CreateEntity = model => new CooperativeAdmin();
            this.CreateModel = entity => new Core.Models.CooperativeAdmin();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.CooperativeAdmin)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (CooperativeAdmin)e,
                    command,
                    context,
                    _resolver);

                entity.Cooperative = (Cooperative)context.Transformer.ToEntity(
                    model.Cooperative,
                    command,
                    context);
                entity.CooperativeId = entity.Cooperative.Id; //value is required

                entity.User = (User)context.Transformer.ToEntity(
                    model.User,
                    command,
                    context);
                entity.UserId = entity.User.Id; //value is required
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (CooperativeAdmin)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.CooperativeAdmin)m,
                    command,
                    context,
                    _resolver);

                model.Cooperative = context.Transformer.ToModel<Core.Models.Cooperative>(
                    entity.Cooperative,
                    command,
                    context);
                model.User = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.User,
                    command,
                    context);
            };
        }
    }

    public class CooperativeFarmTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public CooperativeFarmTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(CooperativeFarm);
            this.ModelType = typeof(Core.Models.CooperativeFarm);

            this.CreateEntity = model => new CooperativeFarm();
            this.CreateModel = entity => new Core.Models.CooperativeFarm();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.CooperativeFarm)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (CooperativeFarm)e,
                    command,
                    context,
                    _resolver);

                entity.Cooperative = (Cooperative)context.Transformer.ToEntity(
                    model.Cooperative,
                    command,
                    context);
                entity.CooperativeId = entity.Cooperative.Id; //required

                entity.Farm = (Farm)context.Transformer.ToEntity(
                    model.Farm,
                    command,
                    context);
                entity.FarmId = entity.Farm.Id; //required
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (CooperativeFarm)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.CooperativeFarm)m,
                    command,
                    context,
                    _resolver);

                model.Cooperative = context.Transformer.ToModel<Core.Models.Cooperative>(
                    entity.Cooperative,
                    command,
                    context);
                model.Farm = context.Transformer.ToModel<Core.Models.Farm>(
                    entity.Farm,
                    command,
                    context);
            };
        }
    }
}
