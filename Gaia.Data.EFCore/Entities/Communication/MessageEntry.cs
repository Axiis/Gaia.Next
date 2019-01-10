using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Pollux.Common.Contracts;
using Axis.Pollux.Communication.Models;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Communication
{
    /// <summary>
    /// This ideally i want to place in a mongo db. so we can move it later
    /// </summary>
    public class MessageEntry : BaseEntity<Guid>
    {
        /// <summary>
        /// json data
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Payload { get; set; }

        public int Status { get; set; }

        public string Channel { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }

        public Guid? TargetUserId { get; set; }

        [Column(TypeName = "ntext")]
        public string RenderedMessage { get; set; }
        public string RenderedTitle { get; set; }
    }

    public class MessageEntryTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public MessageEntryTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(MessageEntry);
            this.ModelType = typeof(Axis.Pollux.Communication.Models.MessageEntry);

            this.CreateEntity = model => new MessageEntry();
            this.CreateModel = entity => new Axis.Pollux.Communication.Models.MessageEntry();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Communication.Models.MessageEntry) m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (MessageEntry) e,
                    command,
                    context,
                    _resolver);

                entity.Channel = model.Channel;
                entity.DestinationAddress = model.DestinationAddress;

                var serializer = _resolver.Resolve<IDataSerializer>();
                entity.Payload = model.Payload == null
                    ? null
                    : serializer.SerializeData(model.Payload.GetType(), model.Payload).Resolve();

                entity.RenderedMessage = model.RenderedMessage;
                entity.RenderedTitle = model.RenderedTitle;
                entity.SourceAddress = model.SourceAddress;
                entity.Status = model.Status;
                entity.TargetUserId = model.TargetUserId;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (MessageEntry) e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Communication.Models.MessageEntry) m,
                    command,
                    context,
                    _resolver);

                model.Channel = entity.Channel;
                model.DestinationAddress = entity.DestinationAddress;
                model.RenderedMessage = entity.RenderedMessage;
                model.RenderedTitle = entity.RenderedTitle;
                model.SourceAddress = entity.SourceAddress;
                model.Status = entity.Status;
                model.TargetUserId = entity.TargetUserId;

                var serializer = _resolver.Resolve<IDataSerializer>();
                model.Payload = string.IsNullOrWhiteSpace(entity.Payload)
                    ? null
                    : serializer.Deserialize<IMessagePayload>(entity.Payload).Resolve();
            };
        }
    }
}
