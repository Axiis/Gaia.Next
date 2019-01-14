using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities
{
    public class Harvest : BaseEntity<Guid>
    {
        public Core.Models.Unit Unit { get; set; }
        public double UnitAmount { get; set; }

        [ForeignKey(nameof(HarvestBatchId))]
        public virtual HarvestBatch Batch { get; set; }
        public Guid HarvestBatchId { get; set; }

        [ForeignKey(nameof(ProduceId))]
        public virtual FarmProduce Produce { get; set; }
        public Guid ProduceId { get; set; }
    }


    public class HarvestTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public HarvestTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Harvest);
            this.ModelType = typeof(Core.Models.Harvest);

            this.CreateEntity = model => new Harvest();
            this.CreateModel = entity => new Core.Models.Harvest();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.Harvest)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Harvest)e,
                    command,
                    context,
                    _resolver);

                entity.Batch = (HarvestBatch) context.Transformer.ToEntity(
                    model.Batch,
                    command,
                    context);
                entity.HarvestBatchId = entity.Batch?.Id ?? default(Guid);
                entity.Produce = (FarmProduce) context.Transformer.ToEntity(
                    model.Produce,
                    command,
                    context);
                entity.ProduceId = entity.Produce?.Id ?? default(Guid);
                entity.Unit = model.Unit;
                entity.UnitAmount = model.UnitAmount;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Harvest)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.Harvest)m,
                    command,
                    context,
                    _resolver);

                model.Batch = context.Transformer.ToModel<Core.Models.HarvestBatch>(
                    entity.Batch,
                    command,
                    context);
                model.Produce = context.Transformer.ToModel<Core.Models.FarmProduce>(
                    entity.Produce,
                    command,
                    context);
            };
        }
    }
}
