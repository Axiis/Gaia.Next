using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class UserData: BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }
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

    public class UserDataTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public UserDataTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(UserData);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.UserData);

            this.CreateEntity = model => new UserData();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.UserData();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.UserData)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (UserData)e,
                    command,
                    context,
                    _resolver);

                entity.Name = model.Name;
                entity.Data = model.Data;
                entity.Type = model.Type;
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
                var entity = (UserData)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.UserData)m,
                    command,
                    context,
                    _resolver);

                model.Name = entity.Name;
                model.Data = entity.Data;
                model.Type = entity.Type;
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
