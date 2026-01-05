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

        [Fact]
        public void GetAllReindeers_ReturnsAllReindeers()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new SantaService(context);

            context.Reindeers.AddRange(
                new Reindeer { Name = "Dasher", PlateNumber = "XMAS-002", Weight = 110, Packets = 45 },
                new Reindeer { Name = "Dancer", PlateNumber = "XMAS-003", Weight = 115, Packets = 48 }
            );
            context.SaveChanges();

            // Act
            var result = service.GetAllReindeers();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetReindeerById_ExistingId_ReturnsReindeer()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var service = new SantaService(context);
            var reindeer = new Reindeer { Name = "Prancer", PlateNumber = "XMAS-004", Weight = 105, Packets = 40 };
            context.Reindeers.Add(reindeer);
            context.SaveChanges();

            // Act
            var result = service.GetReindeerById(reindeer.Id);

            // Assert
            Assert.Equal("Prancer", result.Name);
        }

    }
}
