using Cinvidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Context.Initializer;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services
{
    public class SantaServiceTest
    {
        protected readonly SantaDbContext santaDb;
        public SantaServiceTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<SantaDbContext>()
                .UseSqlite(connection)
                .Options;

            santaDb = new SantaDbContext(options);
            santaDb.Database.EnsureCreated();
            using var context = DbInitializer.InitializeDatabase(santaDb);
        }


        [Fact]
        public void GetAllChildren_ReturnsAllChildren()
        {
            // Arrange
            var service = new SantaService(santaDb);

            // Act
            var result = service.GetAllChildren();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void GetNaughtyChildren_ReturnsNaughtyChildren()
        {
            // Arrange
            var service = new SantaService(santaDb);

            // Act
            var result = service.GetNaughtyChildren();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void GetFailureDeliveries_ReturnsOnlyFailedDeliveries()
        {
            // Arrange
            var service = new SantaService(santaDb);

            // Act
            var result = service.GetFailureDeliveries();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, d => Assert.Equal(DeliveryStatus.Failed, d.Status));
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void GetWishlistByChildId_ReturnsCorrectWishes()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var childId = santaDb.Children.First(c => c.Name == "Sophia").Id;

            // Act
            var result = service.GetWishlistByChildId(childId);

            // Assert
            Assert.NotNull(result);
            Assert.All(result, wish => Assert.Equal(childId, wish.ChildId));
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void GetWishlistByChildIdOrderedByPriority_ReturnsDescendingOrder()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var childId = santaDb.Wishes.First().ChildId;

            // Act
            var result = service.GetWishlistByChildIdOrderedByPriority(childId);

            // Assert
            Assert.NotNull(result);
            var priorities = result.Select(w => w.Priority).ToList();
            var expectedDescending = priorities.OrderByDescending(p => p).ToList();
            Assert.Equal(expectedDescending, priorities);
        }
    }
}

