using Microsoft.EntityFrameworkCore;

namespace Identity.Repository
{
    public class IdentityDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> Claims { get; set; }
        public virtual DbSet<UserExternalLogin> ExternalLogins { get; set; }
        public virtual DbSet<UserToken> Tokens { get; set; }
        public virtual DbSet<UserOneTimePassword> OneTimePasswords { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            AddUser(builder);
            AddUserClaim(builder);
            AddUserExternalLogin(builder);
            AddUserToken(builder);
            AddUserOneTimePassword(builder);

            base.OnModelCreating(builder);
        }
        
           private static void AddUser(ModelBuilder builder)
        {
            builder.Entity<User>()
                .Property(item => item.Username)
                .HasMaxLength(256)
                .IsRequired();

            builder.Entity<User>()
                .Property(item => item.Email)
                .HasMaxLength(320);
            builder.Entity<User>()
                .Property(item => item.PhoneNumber)
                .HasMaxLength(32);

            builder.Entity<User>()
                .Property(item => item.PasswordHash)
                .HasMaxLength(450);
            builder.Entity<User>()
                .Property(item => item.SecurityStamp)
                .HasMaxLength(450);
        }

        private static void AddUserClaim(ModelBuilder builder)
        {
            builder.Entity<UserClaim>()
                .Property(item => item.Type)
                .HasMaxLength(320)
                .IsRequired();

            builder.Entity<UserClaim>()
                .Property(item => item.Value)
                .HasMaxLength(450)
                .IsRequired();
        }

        private static void AddUserExternalLogin(ModelBuilder builder)
        {
            builder.Entity<UserExternalLogin>()
                .Property(item => item.Provider)
                .HasMaxLength(450)
                .IsRequired();

            builder.Entity<UserExternalLogin>()
                .Property(item => item.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Entity<UserExternalLogin>()
                .Property(item => item.Key)
                .HasMaxLength(450)
                .IsRequired();
        }
        
        private static void AddUserToken(ModelBuilder builder)
        {
            builder.Entity<UserToken>()
                .Property(item => item.Name)
                .HasMaxLength(256)
                .IsRequired();
            
            builder.Entity<UserToken>()
                .Property(item => item.Key)
                .HasMaxLength(450)
                .IsRequired();
        }
        
        private static void AddUserOneTimePassword(ModelBuilder builder)
        {
            builder.Entity<UserOneTimePassword>()
                .Property(item => item.Key)
                .HasMaxLength(450)
                .IsRequired();
        }
    }
}
