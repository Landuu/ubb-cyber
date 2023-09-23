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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("Database"));
        }

        public DbSet<User> Users { get; set; }
    }
}
