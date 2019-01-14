using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities
{
    public class HarvestBatch : BaseEntity<Guid>
    {
        public string BatchTitle { get; set; }
        public DateTimeOffset BatchDate { get; set; }
        public Core.Models.HarvestBatchStatus Status { get; set; }

        [ForeignKey(nameof(FarmId))]
        public virtual Farm Farm { get; set; }
        public Guid FarmId { get; set; }

        public virtual ICollection<Harvest> Harvests { get; set; } = new HashSet<Harvest>();
    }


    public class HarvestBatchTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public HarvestBatchTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(HarvestBatch);
            this.ModelType = typeof(Core.Models.HarvestBatch);

            this.CreateEntity = model => new HarvestBatch();
            this.CreateModel = entity => new Core.Models.HarvestBatch();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Core.Models.HarvestBatch)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (HarvestBatch)e,
                    command,
                    context,
                    _resolver);

                entity.Farm = (Farm) context.Transformer.ToEntity(
                    model.Farm,
                    command,
                    context);
                entity.FarmId = entity.Farm?.Id ?? default(Guid);
                entity.BatchDate = model.BatchDate;
                entity.BatchTitle = model.BatchTitle;
                entity.Status = model.Status;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (HarvestBatch)e;
                var model = entity.TransformBase(
                    (Core.Models.HarvestBatch)m,
                    command,
                    context,
                    _resolver);

                model.BatchDate = entity.BatchDate;
                model.BatchTitle = entity.BatchTitle;
                model.Farm = context.Transformer.ToModel<Core.Models.Farm>(
                    entity.Farm,
                    command,
                    context);
                model.Status = entity.Status;
                model.Harvests = entity.Harvests?
                    .Select(harvest => context.Transformer.ToModel<Core.Models.Harvest>(
                        harvest,
                        command,
                        context))
                    .ToArray();
            };
        }
    }
}