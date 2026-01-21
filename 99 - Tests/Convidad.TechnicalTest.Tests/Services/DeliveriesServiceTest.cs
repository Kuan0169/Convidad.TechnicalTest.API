using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class DeliveriesServiceTest
{
    protected readonly SantaDbContext santaDb;

    public DeliveriesServiceTest()
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
    public async Task GetDeliveries_ReturnsAllDeliveries()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        santaDb.Children.Add(child);
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var deliveries = new List<Delivery>
        {
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Pending },
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Delivered }
        };
        santaDb.Deliveries.AddRange(deliveries);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);

        // Act
        var result = await service.GetDeliveriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetFailureDeliveries_ReturnsOnlyFailedDeliveries()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        santaDb.Children.Add(child);
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var deliveries = new List<Delivery>
        {
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Pending },
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Failed },
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Failed }
        };
        santaDb.Deliveries.AddRange(deliveries);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);

        // Act
        var result = await service.GetFailureDeliveriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, delivery => Assert.Equal("Failed", delivery.Status));
    }

    [Fact]
    public async Task GetFailureDeliveries_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        var route = new Route { Name = "North Pole Route", Region = "Arctic" };
        santaDb.Children.Add(child);
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var deliveries = new List<Delivery>
        {
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Pending },
            new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Delivered }
        };
        santaDb.Deliveries.AddRange(deliveries);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);

        // Act
        var result = await service.GetFailureDeliveriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}