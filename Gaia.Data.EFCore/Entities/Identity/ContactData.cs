using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class ContactData : BaseEntity<Guid>
    {
        public string Data { get; set; }
        public string Channel { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public int Status { get; set; }

        /// <summary>
        /// comma separated list of values (with commas' encoded away)
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Tags { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class ContactDataTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public ContactDataTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(ContactData);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.ContactData);

            this.CreateEntity = model => new ContactData();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.ContactData();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.ContactData)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (ContactData)e,
                    command,
                    context,
                    _resolver);

                entity.Channel = model.Channel;
                entity.Data = model.Data;
                entity.IsPrimary = model.IsPrimary;
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
                var entity = (ContactData)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.ContactData)m,
                    command,
                    context,
                    _resolver);

                model.Channel = entity.Channel;
                model.Data = entity.Data;
                model.IsPrimary = entity.IsPrimary;
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