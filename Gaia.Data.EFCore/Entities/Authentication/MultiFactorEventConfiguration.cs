using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Axis.Jupiter;
using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore.Entities.Authentication
{
    public class MultiFactorEventConfiguration : BaseEntity<Guid>
    {
        public TimeSpan ValidityDuration { get; set; }
        public string EventLabel { get; set; }

        /// <summary>
        /// comma separated list of values (with commas' encoded away)
        /// </summary>
        [Column(TypeName = "ntext")]
        public string CommunicationChannels { get; set; }
    }

    public class MultiFactorEventConfigurationTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public MultiFactorEventConfigurationTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(MultiFactorEventConfiguration);
            this.ModelType = typeof(Axis.Pollux.Authentication.Models.MultiFactorEventConfiguration);

            this.CreateEntity = model => new MultiFactorEventConfiguration();
            this.CreateModel = entity => new Axis.Pollux.Authentication.Models.MultiFactorEventConfiguration();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Authentication.Models.MultiFactorEventConfiguration) m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (MultiFactorEventConfiguration) e,
                    command,
                    context,
                    _resolver);

                entity.CommunicationChannels = model.CommunicationChannels
                    ?.Select(Encode)
                    .JoinUsing(",");
                entity.EventLabel = model.EventLabel;
                entity.ValidityDuration = model.ValidityDuration;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (MultiFactorEventConfiguration)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Authentication.Models.MultiFactorEventConfiguration)m,
                    command,
                    context,
                    _resolver);

                model.CommunicationChannels = entity
                    .CommunicationChannels?
                    .Split(',')
                    .Select(Decode)
                    .ToArray() ?? new string[0];
                model.EventLabel = entity.EventLabel;
                model.ValidityDuration = entity.ValidityDuration;
            };
        }

        private static string Encode(string part) => part?.Replace(",", "@comma;");
        private static string Decode(string part) => part?.Replace("@comma;", ",");
    }
}
