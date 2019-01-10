using Gaia.Data.EFCore.Entities.Authorization;
using Gaia.Data.EFCore.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Contexts
{
    public class DomainContext: DbContext
    {
        public DomainContext()
        {
        }

        public DomainContext(DbContextOptions<DomainContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        #region Entities

        #region Authentication
        #endregion

        #region Authorization
        #endregion

        #region Communication
        #endregion

        #region Identity
        public DbSet<AddressData> AddressData { get; set; }
        public DbSet<BioData> BioData { get; set; }
        public DbSet<ContactData> ContactData { get; set; }
        public DbSet<NameData> NameData { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserData { get; set; }
        #endregion

        #region Logon
        #endregion

        #region Gaia
        #endregion
        
        #endregion
    }
}
