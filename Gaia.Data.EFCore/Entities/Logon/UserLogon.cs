using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
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
}
