using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Common;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities
{
    public class AppConfiguration: BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Scope { get; set; }
        public string Data { get; set; }
        public CommonDataType Type { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual AppConfiguration Parent { get; set; }
        public Guid ParentId { get; set; }
        public ICollection<AppConfiguration> Children { get; set; } = new HashSet<AppConfiguration>();
    }
    
    public class AppConfigurationTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public AppConfigurationTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(AppConfiguration);
            this.ModelType = typeof(Core.Models.AppConfiguration);

            this.CreateEntity = model => new AppConfiguration();
            this.CreateModel = entity => new Core.Models.AppConfiguration();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.AppConfiguration)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (AppConfiguration)e,
                    command,
                    context,
                    _resolver);

                entity.Data = model.Data;
                entity.Name = model.Name;
                entity.Scope = model.Scope;
                entity.Type = model.Type;
                entity.Parent = (AppConfiguration) context.Transformer.ToEntity(
                    model.Parent, 
                    command, 
                    context);
                entity.ParentId = entity.Parent?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (AppConfiguration)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.AppConfiguration)m,
                    command,
                    context,
                    _resolver);

                model.Data = entity.Data;
                model.Name = entity.Name;
                model.Scope = entity.Scope;
                model.Type = entity.Type;
                model.Parent = context.Transformer.ToModel<Core.Models.AppConfiguration>(
                    entity.Parent,
                    command,
                    context);
                model.Children = entity.Children
                    .Select(config => context.Transformer.ToModel<Core.Models.AppConfiguration>(
                        config,
                        command,
                        context))
                    .ToArray();
            };
        }
    }

}