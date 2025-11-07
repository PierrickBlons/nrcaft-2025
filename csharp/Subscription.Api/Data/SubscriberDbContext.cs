using Microsoft.EntityFrameworkCore;
using Subscription.Api.Services;

namespace Subscription.Api.Data
{
    public class SubscriberDbContext(DbContextOptions<SubscriberDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Subscriber> Subscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscriber>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
