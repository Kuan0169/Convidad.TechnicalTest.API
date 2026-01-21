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
}