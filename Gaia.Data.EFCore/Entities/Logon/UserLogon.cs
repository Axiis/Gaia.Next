using System;
using System.ComponentModel.DataAnnotations.Schema;
using Axis.Jupiter;
using Axis.Proteus.Ioc;
using Gaia.Data.EFCore.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Entities.Logon
{
    public class UserLogon : BaseEntity<Guid>
    {
        public virtual UserAgent Client { get; set; } = new UserAgent();

        public string IpAddress { get; set; }

        public string SessionToken { get; set; }
        public bool Invalidated { get; set; }

        /// <summary>
        /// Represents an offset in minutes that needs to be subtracted from the LOCAL time to get UTC time.
        /// UTC +1h will have a value of +60. NewYork will have a value of -300.
        /// </summary>
        public int TimeZoneOffset { get; set; }

        public string Locale { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }

    [Owned]
    public class UserAgent
    {
        public string OS { get; set; }
        public string OSVersion { get; set; }

        public string Browser { get; set; }
        public string BrowserVersion { get; set; }

        public string Device { get; set; }
    }


    public class UserLogonTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public UserLogonTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(UserLogon);
            this.ModelType = typeof(Axis.Pollux.Logon.Models.UserLogon);

            this.CreateEntity = model => new UserLogon();
            this.CreateModel = entity => new Axis.Pollux.Logon.Models.UserLogon();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Logon.Models.UserLogon)m;
                model.Id = command == TransformCommand.Add ? Guid.NewGuid() : model.Id;
                var entity = model.TransformBase(
                    (UserLogon)e,
                    command,
                    context,
                    _resolver);

                entity.Invalidated = model.Invalidated;
                entity.IpAddress = model.IpAddress;
                entity.Locale = model.Locale;
                entity.SessionToken = model.SessionToken;
                entity.TimeZoneOffset = model.TimeZoneOffset;
                entity.Client = (UserAgent)context.Transformer.ToEntity(
                    model.Client, 
                    command, 
                    context);

                entity.User = (User) context.Transformer.ToEntity(
                    model.User,
                    command,
                    context);
                entity.UserId = entity.User?.Id ?? default(Guid);
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (UserLogon)e;
                var model = entity.TransformBase(
                    (Axis.Pollux.Logon.Models.UserLogon)m,
                    command,
                    context,
                    _resolver);

                model.Invalidated = entity.Invalidated;
                model.IpAddress = entity.IpAddress;
                model.Locale = entity.Locale;
                model.SessionToken = entity.SessionToken;
                model.TimeZoneOffset = entity.TimeZoneOffset;
                model.Client = context.Transformer.ToModel<Axis.Pollux.Logon.Models.UserAgent>(
                    entity.Client,
                    command,
                    context);

                model.User = context.Transformer.ToModel<Axis.Pollux.Identity.Models.User>(
                    entity.User, 
                    command, 
                    context);
            };
        }
    }

    public class UserAgentTransform : ModelTransform
    {
        private readonly IServiceResolver _resolver;

        public UserAgentTransform(IServiceResolver resolver)
        {
            _resolver = resolver;

            this.EntityType = typeof(UserAgent);
            this.ModelType = typeof(Axis.Pollux.Logon.Models.UserAgent);

            this.CreateEntity = model => new UserAgent();
            this.CreateModel = entity => new Axis.Pollux.Logon.Models.UserAgent();

            this.ModelToEntity = (m, e, command, context) =>
            {
                var model = (Axis.Pollux.Logon.Models.UserAgent)m;
                var entity = (UserAgent) e;

                entity.Browser = model.Browser;
                entity.BrowserVersion = model.BrowserVersion;
                entity.Device = model.Device;
                entity.OS = model.OS;
                entity.OSVersion = model.OSVersion;
            };

            this.EntityToModel = (e, m, command, context) =>
            {
                var entity = (UserAgent)e;
                var model = (Axis.Pollux.Logon.Models.UserAgent) m;

                model.Browser = entity.Browser;
                model.BrowserVersion = entity.BrowserVersion;
                model.Device = entity.Device;
                model.OS = entity.OS;
                model.OSVersion = entity.OSVersion;
            };
        }
    }

}
