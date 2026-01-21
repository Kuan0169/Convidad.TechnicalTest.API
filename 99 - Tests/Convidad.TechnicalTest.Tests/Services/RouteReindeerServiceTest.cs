using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class RouteReindeerServiceTest
{
    protected readonly SantaDbContext santaDb;

    public RouteReindeerServiceTest()
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
    public async Task AssignReindeerToRoute_ValidIds_CreatesAssignment()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RouteReindeerService(santaDb);

        // Act
        await service.AssignReindeerToRouteAsync(route.Id, reindeer.Id, 15);

        // Assert
        var assignment = await santaDb.RouteReindeers
            .Where(rr => rr.RouteId == route.Id && rr.ReindeerId == reindeer.Id)
            .FirstOrDefaultAsync();
        Assert.NotNull(assignment);
        Assert.Equal(15, assignment.MaxDeliveries);
        Assert.Equal(0, assignment.CurrentDeliveries);
    }

    [Fact]
    public async Task RemoveReindeerFromRoute_ValidIds_RemovesAssignment()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RouteReindeerService(santaDb);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer.Id, 10);

        // Act
        await service.RemoveReindeerFromRouteAsync(route.Id, reindeer.Id);

        // Assert
        var assignment = await santaDb.RouteReindeers
            .Where(rr => rr.RouteId == route.Id && rr.ReindeerId == reindeer.Id)
            .FirstOrDefaultAsync();
        Assert.Null(assignment);
    }

    [Fact]
    public async Task CanHandleNewDelivery_ValidRouteAtCapacity_ReturnsFalse()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RouteReindeerService(santaDb);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer.Id, 5);

        // Manually set current deliveries to max capacity
        var assignment = await santaDb.RouteReindeers
            .Where(rr => rr.RouteId == route.Id && rr.ReindeerId == reindeer.Id)
            .FirstAsync();
        assignment.CurrentDeliveries = 5;
        await santaDb.SaveChangesAsync();

        // Act
        var result = await service.CanHandleNewDeliveryAsync(route.Id);

        // Assert
        Assert.False(result);
    }
}