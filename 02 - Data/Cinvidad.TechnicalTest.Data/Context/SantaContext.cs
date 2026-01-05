using Cinvidad.TechnicalTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Convidad.TechnicalTest.Data.Context
{
    public class SantaDbContext : DbContext
    {
        public SantaDbContext(DbContextOptions<SantaDbContext> options) : base(options) { }

        public DbSet<Child> Children => Set<Child>();
        public DbSet<Wish> Wishes => Set<Wish>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Child -> Wishes (1:N)
            modelBuilder.Entity<Wish>()
                .HasOne(w => w.Child);

            // Child -> Deliveries (1:N)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Child);

            // Route -> Deliveries (1:N)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Route);


            modelBuilder.Entity<Delivery>()
                .HasIndex(d => new { d.ChildId, d.ScheduledFor })
                .IsUnique(false);
        }
    }
}
