using Microsoft.EntityFrameworkCore;

using NeatPath.Models;
using NeatPath.Models.Interfaces;

namespace NeatPath.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Url> Urls { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ITrackable);

            foreach (var entry in entries)
            {
                var trackable = (ITrackable)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    trackable.CreatedAt = DateTime.UtcNow;
                    trackable.UpdatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    trackable.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<URLData>()
            //    .HasOne(ud => ud.User)
            //    .WithMany(u => u.Urls)
            //    .HasForeignKey<User>(ud => ud.)

            modelBuilder.Entity<Url>()
                .Property(e => e.ClickCount)
                .HasDefaultValue(0);
        }
    }
}
