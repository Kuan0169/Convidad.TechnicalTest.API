using Convidad.TechnicalTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Data.Context
{
    public class SantaDbContext : DbContext
    {
        public SantaDbContext(DbContextOptions<SantaDbContext> options) : base(options) { }

        public DbSet<Child> Children => Set<Child>();
        public DbSet<Wish> Wishes => Set<Wish>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();
        public DbSet<Reindeer> Reindeers => Set<Reindeer>();
        public DbSet<RouteReindeer> RouteReindeers => Set<RouteReindeer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wish>()
                .HasOne(w => w.Child)
                .WithMany(c => c.Wishes)
                .HasForeignKey(w => w.ChildId);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Child)
                .WithMany(c => c.Deliveries)
                .HasForeignKey(d => d.ChildId);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Route)
                .WithMany(r => r.Deliveries)
                .HasForeignKey(d => d.RouteId);

            modelBuilder.Entity<RouteReindeer>(entity =>
            {
                entity.HasKey(rr => new { rr.RouteId, rr.ReindeerId });

                entity.HasOne(rr => rr.Route)
                    .WithMany(r => r.AssignedReindeers)
                    .HasForeignKey(rr => rr.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);
 
                entity.HasOne(rr => rr.Reindeer)
                    .WithMany(r => r.AssignedRoutes)
                    .HasForeignKey(rr => rr.ReindeerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Delivery>()
                .HasIndex(d => new { d.ChildId, d.ScheduledFor })
                .IsUnique(false);
        }
    }
}
