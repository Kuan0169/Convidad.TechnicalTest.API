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
        protected readonly SantaDbContext _dbContext;

        public ReindeerServiceTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<SantaDbContext>()
                .UseSqlite(connection)
                .Options;
            _dbContext = new SantaDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public void AddReindeer_AddsReindeerToDatabase()
        {
            // Arrange
            var service = new SantaService(_dbContext);
            var reindeer = new Reindeer
            {
                Name = "Rudolph",
                PlateNumber = "XMAS-001",
                Weight = 120.5,
                Packets = 50
            };

            // Act
            service.AddReindeer(reindeer);
            var saved = _dbContext.Reindeers.First(r => r.Name == "Rudolph");

            // Assert
            Assert.NotNull(saved);
            Assert.Equal("XMAS-001", saved.PlateNumber);
            Assert.Equal(120.5, saved.Weight);
        }

        [Fact]
        public void AssignReindeerToDelivery_AssignsCorrectReindeer()
        {
            // Arrange
            var service = new SantaService(_dbContext);
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

            _dbContext.Children.Add(child);
            _dbContext.Routes.Add(route);
            _dbContext.Deliveries.Add(delivery);
            _dbContext.Reindeers.Add(reindeer);
            _dbContext.SaveChanges();

            // Act
            service.AssignReindeerToDelivery(delivery.Id, reindeer.Id);
            var updatedDelivery = _dbContext.Deliveries.First(d => d.Id == delivery.Id);

            // Assert
            Assert.Equal(reindeer.Id, updatedDelivery.ReindeerId);
        }
    }
}
