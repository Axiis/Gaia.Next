using Axis.Jupiter;
using Axis.Jupiter.EFCore;
using Gaia.Data.EFCore.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gaia.Data.EFCore.Stores
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

            modelBuilder
                .Entity<Entities.Authentication.MultiFactorEventConfiguration>()
                .HasIndex(e => e.EventLabel)
                .IsUnique(true);
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

    public class DomainStoreCommand : EFStoreCommand
    {
        public DomainStoreCommand(ModelTransformer transformer, DomainContext context) 
        : base(transformer, context)
        {
        }
    }

    public class DomainStoreQuery : EFStoreQuery
    {
        public DomainStoreQuery(DomainContext context) 
        : base(context)
        {
        }
    }
}
