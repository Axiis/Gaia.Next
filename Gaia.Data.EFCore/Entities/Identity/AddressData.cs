using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Identity
{
    public class AddressData: BaseEntity<Guid>
    {
        public string PostCode { get; set; }
        public string Flat { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }

        public int Status { get; set; }


        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class AddressDataTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public AddressDataTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(AddressData);
            this.ModelType = typeof(Axis.Pollux.Identity.Models.AddressData);

            this.CreateEntity = model => new AddressData();
            this.CreateModel = entity => new Axis.Pollux.Identity.Models.AddressData();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Identity.Models.AddressData)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (AddressData)e,
                    command,
                    context,
                    _resolver);

                entity.City = model.City;
                entity.Country = model.Country;
                entity.Flat = model.Flat;
                entity.PostCode = model.PostCode;
                entity.StateProvince = model.StateProvince;
                entity.Status = model.Status;
                entity.Street = model.Street;

                entity.Owner = (User) context.Transformer.ToEntity(
                    model.Owner, 
                    command, 
                    context);
                entity.OwnerId = entity.Owner?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (AddressData)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Identity.Models.AddressData)m,
                    command,
                    context,
                    _resolver);

                model.City = entity.City;
                model.Country = entity.Country;
                model.Flat = entity.Flat;
                model.PostCode = entity.PostCode;
                model.StateProvince = entity.StateProvince;
                model.Status = entity.Status;
                model.Street = entity.Street;

                model.Owner = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.Owner, 
                    command, 
                    context);
            };
        }
    }
}
