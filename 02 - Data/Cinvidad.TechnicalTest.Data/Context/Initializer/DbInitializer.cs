using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Convidad.TechnicalTest.Data.Context.Initializer
{
    public static class DbInitializer
    {
        public static async Task InitializeDatabase(SantaDbContext db)
        {
            await db.Database.EnsureCreatedAsync();

            if (db.Children.Any())
                return;

            // ---------- Routes ----------
            var routeEs = new Route
            {
                Name = "Iberia Route",
                Region = "ES",
                CapacityPerNight = 500
            };

            var routeEu = new Route
            {
                Name = "EU Central Route",
                Region = "EU",
                CapacityPerNight = 700
            };

            var routeUs = new Route
            {
                Name = "US East Route",
                Region = "US-EAST",
                CapacityPerNight = 800
            };

            db.Routes.AddRange(routeEs, routeEu, routeUs);

            // ---------- Children ----------
            var lucia = new Child
            {
                Name = "Lucía",
                CountryCode = "ES",
                IsNice = true
            };

            var nil = new Child
            {
                Name = "Nil",
                CountryCode = "ES",
                IsNice = true
            };

            var emma = new Child
            {
                Name = "Emma",
                CountryCode = "FR",
                IsNice = true
            };

            var oliver = new Child
            {
                Name = "Oliver",
                CountryCode = "UK",
                IsNice = false
            };

            var sophia = new Child
            {
                Name = "Sophia",
                CountryCode = "US",
                IsNice = true
            };

            db.Children.AddRange(lucia, nil, emma, oliver, sophia);

            // ---------- Wishes ----------
            db.Wishes.AddRange(

                new Wish { Child = lucia, Category = WishCategory.Toy, Priority = 5 },
                new Wish { Child = lucia, Category = WishCategory.Book, Priority = 3 },

                new Wish { Child = nil, Category = WishCategory.Game, Priority = 4 },
                new Wish { Child = nil, Category = WishCategory.Toy, Priority = 2 },

                new Wish { Child = emma, Category = WishCategory.Clothes, Priority = 3 },
                new Wish { Child = emma, Category = WishCategory.Book, Priority = 4 },

                new Wish { Child = oliver, Category = WishCategory.Other, Priority = 1 },

                new Wish { Child = sophia, Category = WishCategory.Toy, Priority = 5 },
                new Wish { Child = sophia, Category = WishCategory.Game, Priority = 4 },
                new Wish { Child = sophia, Category = WishCategory.Book, Priority = 2 }
            );

            // ---------- Deliveries ----------
            db.Deliveries.AddRange(

                new Delivery
                {
                    Child = lucia,
                    Route = routeEs,
                    ScheduledFor = new DateTime(2025, 12, 20),
                    Status = DeliveryStatus.Delivered,
                    DeliveredAt = DateTimeOffset.UtcNow.AddDays(-2)
                },

                new Delivery
                {
                    Child = nil,
                    Route = routeEs,
                    ScheduledFor = new DateTime(2025, 12, 21),
                    Status = DeliveryStatus.InTransit
                },

                new Delivery
                {
                    Child = emma,
                    Route = routeEu,
                    ScheduledFor = new DateTime(2025, 12, 21),
                    Status = DeliveryStatus.Failed,
                    FailureReason = FailureReason.AddressNotFound,
                    FailureDetails = "Missing apartment number"
                },

                new Delivery
                {
                    Child = oliver,
                    Route = routeEu,
                    ScheduledFor = new DateTime(2025, 12, 22),
                    Status = DeliveryStatus.Pending
                },

                new Delivery
                {
                    Child = sophia,
                    Route = routeUs,
                    ScheduledFor = new DateTime(2025, 12, 23),
                    Status = DeliveryStatus.Pending
                }
            );

            await db.SaveChangesAsync();
        }
    }
}
