using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class BioData : BaseEntity<Guid>
    {
        public DateTimeOffset? DateOfBirth { get; set; }
        public string Gender { get; set; }

        public string CountryOfBirth { get; set; }


        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class BioDataTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public BioDataTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(BioData);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.BioData);

            this.CreateEntity = model => new BioData();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.BioData();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.BioData)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (BioData)e,
                    command,
                    context,
                    _resolver);

                entity.CountryOfBirth = model.CountryOfBirth;
                entity.DateOfBirth = model.DateOfBirth;
                entity.Gender = model.Gender;

                entity.Owner = (User) context.Transformer.ToEntity(
                    model.Owner,
                    command,
                    context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (BioData)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.BioData)m,
                    command,
                    context,
                    _resolver);

                model.CountryOfBirth = entity.CountryOfBirth;
                model.DateOfBirth = entity.DateOfBirth;
                model.Gender = entity.Gender;

                model.Owner = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.Owner, 
                    command, 
                    context);
            };
        }
    }
}
