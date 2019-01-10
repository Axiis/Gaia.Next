using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Pollux.Authorization.Abac.Models;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Authorization
{
    /// <summary>
    /// Should be moved to mongo
    /// </summary>
    public class Policy: BaseEntity<Guid>
    {
        [Column(TypeName = "ntext")]
        public string GovernedResources { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual Policy Parent { get; set; }
        public Guid ParentId { get; set; }
        public virtual ICollection<Policy> SubPolicies { get; set; } = new HashSet<Policy>();
    }

    public class PolicyTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public PolicyTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Policy);
            this.ModelType = typeof(PolluxPolicy);

            this.CreateEntity = model => new Policy();
            this.CreateModel = entity => new PolluxPolicy();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (PolluxPolicy) m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Policy) e,
                    command,
                    context,
                    _resolver);

                entity.Code = model.Code;
                entity.GovernedResources = model.GovernedResources
                    ?.Select(Encode)
                    .JoinUsing(",");
                entity.Title = model.Title;

                entity.Parent = (Policy) context.Transformer.ToEntity(model.Parent, command, context);
                entity.ParentId = entity.Parent?.Id ?? Guid.Empty;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Policy) e;
                var model = entity.TransformBase(
                    (PolluxPolicy) m,
                    command,
                    context,
                    _resolver);

                model.Code = entity.Code;
                model.GovernedResources = entity.GovernedResources
                    ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Decode)
                    .ToArray();
                model.Title = entity.Title;

                model.Parent = context.Transformer.ToModel<PolluxPolicy>(entity.Parent, command, context);
                model.SubPolicies = entity.SubPolicies
                    .Select(policy => context.Transformer.ToModel<PolluxPolicy>(policy, command, context))
                    //somewhere here, bind policies to their rules from the Gaia.Core.AuthorizationPolicies Assemblies
                    .ToArray();
            };
        }

        private static string Encode(string part) => part?.Replace(",", "@comma;");
        private static string Decode(string part) => part?.Replace("@comma;", ",");
    }
}
