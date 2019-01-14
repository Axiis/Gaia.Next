using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;
using Gaia.Core.Utils;

namespace Gaia.Data.EFCore.Entities
{
    public class Farm : BaseEntity<Guid>
    {
        [ForeignKey(nameof(OwnerId))]
        public Farmer Owner { get; set; }
        public Guid OwnerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LocationDescription { get; set; }

        [Column(TypeName = "ntext")]
        public string Area { get; set; }

        public virtual ICollection<Cooperative> Cooperatives { get; set; } = new HashSet<Cooperative>();
        public virtual ICollection<ProductCategory> Products { get; set; } = new HashSet<ProductCategory>();
        public virtual ICollection<HarvestBatch> Harvests { get; set; } = new HashSet<HarvestBatch>();
    }

    public class ProductCategory : BaseEntity<Guid>
    {
        [ForeignKey(nameof(FarmProduceId))]
        public virtual FarmProduce FarmProduce { get; set; }
        public Guid FarmProduceId { get; set; }

        [ForeignKey(nameof(FarmId))]
        public virtual Farm Farm { get; set; }
        public Guid FarmId { get; set; }
    }


    public class FarmTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public FarmTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(Farm);
            this.ModelType = typeof(Core.Models.Farm);

            this.CreateEntity = model => new Farm();
            this.CreateModel = entity => new Core.Models.Farm();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.Farm)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (Farm)e,
                    command,
                    context,
                    _resolver);

                entity.Area = model.Area
                    ?.Select(position => position.ToString())
                    .JoinUsing(";") ?? "";
                entity.Description = model.Description;
                entity.LocationDescription = model.LocationDescription;
                entity.Name = model.Name;

                entity.Owner = (Farmer)context.Transformer.ToEntity(
                    model.Owner,
                    command,
                    context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (Farm)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.Farm)m,
                    command,
                    context,
                    _resolver);

                model.Area = entity.Area
                    ?.Split(';')
                    .Select(GeoPosition.Parse)
                    .ToArray() ?? new GeoPosition[0];
                model.Description = entity.Description;
                model.LocationDescription = entity.LocationDescription;
                model.Name = entity.Name;

                model.Owner = context.Transformer.ToModel<Core.Models.Farmer>(
                    entity.Owner,
                    command,
                    context);

                model.Cooperatives = entity.Cooperatives?
                    .Select(coop => context.Transformer.ToModel<Core.Models.CooperativeFarm>(
                        coop,
                        command,
                        context))
                    .ToArray() ?? new Core.Models.CooperativeFarm[0];

                model.Harvests = entity.Harvests?
                    .Select(coop => context.Transformer.ToModel<Core.Models.HarvestBatch>(
                        coop,
                        command,
                        context))
                    .ToArray() ?? new Core.Models.HarvestBatch[0];

                model.Products = entity.Products?
                    .Select(coop => context.Transformer.ToModel<Core.Models.ProductCategory>(
                        coop,
                        command,
                        context))
                    .ToArray() ?? new Core.Models.ProductCategory[0];
            };
        }
    }
    

    public class ProductCategoryTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public ProductCategoryTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(ProductCategory);
            this.ModelType = typeof(Core.Models.ProductCategory);

            this.CreateEntity = model => new ProductCategory();
            this.CreateModel = entity => new Core.Models.ProductCategory();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.ProductCategory)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (ProductCategory)e,
                    command,
                    context,
                    _resolver);

                entity.Farm = (Farm) context.Transformer.ToEntity(
                    model.Farm,
                    command,
                    context);
                entity.FarmId = entity.Farm.Id; //required

                entity.FarmProduce = (FarmProduce) context.Transformer.ToEntity(
                    model.FarmProduce,
                    command,
                    context);
                entity.FarmProduceId = entity.FarmProduce.Id; //required
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (ProductCategory)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.ProductCategory)m,
                    command,
                    context,
                    _resolver);

                model.Farm = context.Transformer.ToModel<Core.Models.Farm>(
                    entity.Farm,
                    command,
                    context);
                model.FarmProduce = context.Transformer.ToModel<Core.Models.FarmProduce>(
                    entity.FarmProduce,
                    command,
                    context);
            };
        }
    }
}