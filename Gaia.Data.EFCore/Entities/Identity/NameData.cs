using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class NameData : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public int Status { get; set; }


        /// <summary>
        /// comma separated list of values (with commas' encoded away)
        /// </summary>
        public string Tags { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class NameDataTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public NameDataTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(NameData);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.NameData);

            this.CreateEntity = model => new NameData();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.NameData();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.NameData)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (NameData)e,
                    command,
                    context,
                    _resolver);

                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.MiddleName = model.MiddleName;
                entity.Status = model.Status;
                entity.Tags = model.Tags
                    ?.Select(Encode)
                    .JoinUsing(",");

                entity.Owner = (User) context.Transformer.ToEntity(
                    model.Owner,
                    command,
                    context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (NameData)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.NameData)m,
                    command,
                    context,
                    _resolver);

                model.FirstName = entity.FirstName;
                model.LastName = entity.LastName;
                model.MiddleName = entity.MiddleName;
                model.LastName = entity.LastName;
                model.Status = entity.Status;
                model.Tags = entity.Tags
                    ?.Split(',')
                    .Select(Decode)
                    .ToArray() ?? new string[0];

                model.Owner = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.Owner, 
                    command, 
                    context);
            };
        }

        private static string Encode(string part) => part?.Replace(",", "@comma;");
        private static string Decode(string part) => part?.Replace("@comma;", ",");
    }
}
