using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class RoutesServiceTest
{
    protected readonly SantaDbContext santaDb;

    public RoutesServiceTest()
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
    public async Task GetAllRoutes_ReturnsAllRoutes()
    {
        // Arrange
        var routes = new List<Route>
        {
            new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 },
            new Route { Name = "Europe Route", Region = "Europe", CapacityPerNight = 30 }
        };
        santaDb.Routes.AddRange(routes);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);

        // Act
        var result = await service.GetAllRoutesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "North Pole Route");
        Assert.Contains(result, r => r.Name == "Europe Route");
    }

    [Fact]
    public async Task GetAllRoutes_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var service = new RoutesService(santaDb);

        // Act
        var result = await service.GetAllRoutesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddRoute_ValidRoute_AddsToDatabase()
    {
        // Arrange
        var service = new RoutesService(santaDb);
        var createRouteDto = new CreateRouteDto("Asia Route", "Asia", 40);

        // Act
        var result = await service.AddRouteAsync(createRouteDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Asia Route", result.Name);
        Assert.Equal("Asia", result.Region);
        Assert.Equal(40, result.CapacityPerNight);

        // Verify it was saved to database
        var savedRoute = await santaDb.Routes.FindAsync(result.Id);
        Assert.NotNull(savedRoute);
        Assert.Equal("Asia Route", savedRoute.Name);
    }

    [Fact]
    public async Task DeleteRoute_ValidId_RemovesFromDatabase()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);

        // Act
        await service.DeleteRouteAsync(route.Id);

        // Assert
        var deletedRoute = await santaDb.Routes.FindAsync(route.Id);
        Assert.Null(deletedRoute);
    }

    [Fact]
    public async Task DeleteRoute_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new RoutesService(santaDb);
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteRouteAsync(invalidId));
    }

    [Fact]
    public async Task AssignReindeerToRoute_ValidIds_CreatesAssignment()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);

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
    public async Task AssignReindeerToRoute_InvalidRouteId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
        var invalidRouteId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AssignReindeerToRouteAsync(invalidRouteId, reindeer.Id, 10));
    }

    [Fact]
    public async Task AssignReindeerToRoute_InvalidReindeerId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
        var invalidReindeerId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AssignReindeerToRouteAsync(route.Id, invalidReindeerId, 10));
    }

    [Fact]
    public async Task RemoveReindeerFromRoute_ValidIds_RemovesAssignment()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
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
    public async Task GetReindeersForRoute_ValidRouteId_ReturnsAssignedReindeers()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        var reindeer1 = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        var reindeer2 = new Reindeer { Name = "Blitzen", PlateNumber = "XMAS-002", Weight = 95.0, Packets = 45 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.AddRange(new[] { reindeer1, reindeer2 });
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer1.Id, 10);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer2.Id, 8);

        // Act
        var result = await service.GetReindeersForRouteAsync(route.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Rudolph");
        Assert.Contains(result, r => r.Name == "Blitzen");
    }

    [Fact]
    public async Task GetReindeersForRoute_InvalidRouteId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new RoutesService(santaDb);
        var invalidRouteId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetReindeersForRouteAsync(invalidRouteId));
    }

    [Fact]
    public async Task CanHandleNewDelivery_ValidRouteWithCapacity_ReturnsTrue()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer.Id, 10);

        // Act
        var result = await service.CanHandleNewDeliveryAsync(route.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanHandleNewDelivery_ValidRouteAtCapacity_ReturnsFalse()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        var reindeer = new Reindeer { Name = "Rudolph", PlateNumber = "XMAS-001", Weight = 100.0, Packets = 50 };
        santaDb.Routes.Add(route);
        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        var service = new RoutesService(santaDb);
        await service.AssignReindeerToRouteAsync(route.Id, reindeer.Id, 5);

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