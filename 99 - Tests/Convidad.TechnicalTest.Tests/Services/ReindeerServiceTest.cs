using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Services.SantaService;
using Convidad.TechnicalTest.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services
{
    public class ReindeerServiceTest
    {
        public SantaDbContext CreateInMemoryDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<SantaDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new SantaDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void AddReindeer_ValidReindeer_AddsToDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new SantaService(context);
            var reindeer = new Reindeer
            {
                Name = "Rudolph",
                PlateNumber = "XMAS-001",
                Weight = 120.5,
                Packets = 50
            };

            // Act
            service.AddReindeer(reindeer);
            var saved = context.Reindeers.Find(reindeer.Id);

            // Assert
            Assert.NotNull(saved);
            Assert.Equal("Rudolph", saved.Name);
            Assert.Equal("XMAS-001", saved.PlateNumber);
            Assert.Equal(120.5, saved.Weight);
            Assert.Equal(50, saved.Packets);
        }

    }
}
