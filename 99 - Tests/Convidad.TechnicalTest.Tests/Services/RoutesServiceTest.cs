using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
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
}