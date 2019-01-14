using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities
{
    public class FarmProduce : BaseEntity<Guid>
    {
        public string CommonName { get; set; }
        public string BotanicalName { get; set; }
        public string LocalName { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        //other info can eventually come here
    }


    public class FarmProduceTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public FarmProduceTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(FarmProduce);
            this.ModelType = typeof(Core.Models.FarmProduce);

            this.CreateEntity = model => new FarmProduce();
            this.CreateModel = entity => new Core.Models.FarmProduce();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Gaia.Core.Models.FarmProduce)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (FarmProduce)e,
                    command,
                    context,
                    _resolver);

                entity.Description = model.Description;
                entity.BotanicalName = model.BotanicalName;
                entity.CommonName = model.CommonName;
                entity.LocalName = model.LocalName;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (FarmProduce)e;
                var model = entity.TransformBase(
                    (Gaia.Core.Models.FarmProduce)m,
                    command,
                    context,
                    _resolver);

                model.Description = entity.Description;
                model.BotanicalName = entity.BotanicalName;
                model.CommonName = entity.CommonName;
                model.LocalName = entity.LocalName;
            };
        }
    }
}
