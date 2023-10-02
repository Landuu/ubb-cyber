using Microsoft.EntityFrameworkCore;
using ubb_cyber.Models;

namespace ubb_cyber.Database
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UsedPassword> UsedPasswords { get; set; }
        public DbSet<PasswordPolicy> PasswordPolicies { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("Database"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();
        }
    }
}
