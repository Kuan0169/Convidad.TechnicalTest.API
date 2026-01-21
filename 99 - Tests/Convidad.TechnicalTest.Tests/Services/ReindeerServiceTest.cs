using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class ReindeersServiceTest
{
    protected readonly SantaDbContext santaDb;

    public ReindeersServiceTest()
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
    public async Task GetAllReindeers_ReturnsAllReindeers()
    {
        // Arrange
        var reindeers = new List<Reindeer>
        {
            new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 },
            new Reindeer { Name = "Blitzen", PlateNumber = "XMAS-002", Weight = 95.0, Packets = 45 }
        };
        santaDb.Reindeers.AddRange(reindeers);
        await santaDb.SaveChangesAsync();

        var service = new ReindeersService(santaDb);

        // Act
        var result = await service.GetAllReindeersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetReindeerById_ValidId_ReturnsReindeer()
    {
        // Arrange
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new ReindeersService(santaDb);

        // Act
        var result = await service.GetReindeerByIdAsync(reindeer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reindeer.Id, result.Id);
        Assert.Equal("Rudolph", result.Name);
        Assert.Equal("XMAS-001", result.PlateNumber);
        Assert.Equal(100.0, result.Weight);
        Assert.Equal(50, result.Packets);
    }

    [Fact]
    public async Task GetReindeerById_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new ReindeersService(santaDb);
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetReindeerByIdAsync(nonExistentId));
    }

    [Fact]
    public async Task AddReindeer_ValidReindeer_AddsToDatabase()
    {
        // Arrange
        var service = new ReindeersService(santaDb);
        var reindeerDto = new ReindeerDto(Guid.NewGuid(), "Comet", "XMAS-003", 90.0, 40);

        // Act
        var result = await service.AddReindeerAsync(reindeerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Comet", result.Name);
        Assert.Equal("XMAS-003", result.PlateNumber);
        Assert.Equal(90.0, result.Weight);
        Assert.Equal(40, result.Packets);

        var savedReindeer = await santaDb.Reindeers.FindAsync(result.Id);
        Assert.NotNull(savedReindeer);
        Assert.Equal("Comet", savedReindeer.Name);
    }

    [Fact]
    public async Task DeleteReindeer_ValidId_RemovesFromDatabase()
    {
        // Arrange
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new ReindeersService(santaDb);

        // Act
        await service.DeleteReindeerAsync(reindeer.Id);

        // Assert
        var deletedReindeer = await santaDb.Reindeers.FindAsync(reindeer.Id);
        Assert.Null(deletedReindeer);
    }

    [Fact]
    public async Task DeleteReindeer_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new ReindeersService(santaDb);
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteReindeerAsync(invalidId));
    }
}