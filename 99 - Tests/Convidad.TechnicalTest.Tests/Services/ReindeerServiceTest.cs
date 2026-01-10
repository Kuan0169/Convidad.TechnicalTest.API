using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services
{
    public class ReindeerServiceTest
    {
        protected readonly SantaDbContext santaDb;
        public ReindeerServiceTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<SantaDbContext>()
                .UseSqlite(connection)
                .Options;
            santaDb = new SantaDbContext(options);
            santaDb.Database.EnsureCreated();
        }

        [Fact]
        public void AddReindeer_AddsReindeerToDatabase()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var reindeer = new Reindeer
            {
                Name = "Rudolph",
                PlateNumber = "XMAS-001",
                Weight = 120.5,
                Packets = 50
            };

            // Act
            service.AddReindeer(reindeer);
            var saved = santaDb.Reindeers.First(r => r.Name == "Rudolph");

            // Assert
            Assert.NotNull(saved);
            Assert.Equal("XMAS-001", saved.PlateNumber);
            Assert.Equal(120.5, saved.Weight);
        }

        [Fact]
        public void AssignReindeerToDelivery_AssignsCorrectReindeer()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var child = new Child 
            {
                Name = "Test", 
                CountryCode = "US" 
            };

            var route = new Route 
            { 
                Name = "Test Route", 
                Region = "North Pole" 
            };

            var delivery = new Delivery 
            { 
                ChildId = child.Id, 
                RouteId = route.Id 
            };

            var reindeer = new Reindeer 
            { 
                Name = "Blitzen", 
                PlateNumber = "XMAS-002", 
                Weight = 110, 
                Packets = 40 
            };

            santaDb.Children.Add(child);
            santaDb.Routes.Add(route);
            santaDb.Deliveries.Add(delivery);
            santaDb.Reindeers.Add(reindeer);
            santaDb.SaveChanges();

            // Act
            service.AssignReindeerToDelivery(delivery.Id, reindeer.Id);
            var updatedDelivery = santaDb.Deliveries.First(d => d.Id == delivery.Id);

            // Assert
            Assert.Equal(reindeer.Id, updatedDelivery.ReindeerId);
        }

        [Fact]
        public void AssignReindeerToDelivery_NonExistingDelivery_ThrowsKeyNotFoundException()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var reindeer = new Reindeer
            {
                Name = "Comet",
                PlateNumber = "XMAS-007",
                Weight = 105,
                Packets = 35
            };
            santaDb.Reindeers.Add(reindeer);
            santaDb.SaveChanges();

            var nonExistingDeliveryId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                service.AssignReindeerToDelivery(nonExistingDeliveryId, reindeer.Id));

            Assert.Contains("Delivery", exception.Message);
        }

        [Fact]
        public void AssignReindeerToDelivery_NonExistingReindeer_ThrowsKeyNotFoundException()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var child = new Child { Name = "Test Child", CountryCode = "TC" };
            var route = new Route { Name = "Test Route", Region = "Test Region" };
            var delivery = new Delivery { ChildId = child.Id, RouteId = route.Id };

            santaDb.Children.Add(child);
            santaDb.Routes.Add(route);
            santaDb.Deliveries.Add(delivery);
            santaDb.SaveChanges();

            var nonExistingReindeerId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                service.AssignReindeerToDelivery(delivery.Id, nonExistingReindeerId));

            Assert.Contains("Reindeer", exception.Message);
        }

        [Fact]
        public void GetReindeerById_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var service = new SantaService(santaDb);
            var nonExistingId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                service.GetReindeerById(nonExistingId));

            Assert.Contains("Reindeer", exception.Message);
        }
    }
}
