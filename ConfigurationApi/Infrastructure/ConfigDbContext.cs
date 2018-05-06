using ConfigurationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationApi.Infrastructure
{
    public class ConfigDbContext : DbContext
    {
        public DbSet<Config> Configurations { get; set; }

        public ConfigDbContext(DbContextOptions<ConfigDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>().ToTable("Configs");
        }
    }
}
