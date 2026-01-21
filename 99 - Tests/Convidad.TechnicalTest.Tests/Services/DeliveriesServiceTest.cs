using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Models.DTOs;
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

    [Fact]
    public async Task AddDelivery_ValidDelivery_AddsToDatabase()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        santaDb.Children.Add(child);
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);
        var createDeliveryDto = new CreateDeliveryDto(child.Id, route.Id);

        // Act
        var result = await service.AddDeliveryAsync(createDeliveryDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(child.Id, result.ChildId);
        Assert.Equal(route.Id, result.RouteId);
        Assert.Equal("Pending", result.Status);

        // Verify it was saved to database
        var savedDelivery = await santaDb.Deliveries.FindAsync(result.Id);
        Assert.NotNull(savedDelivery);
        Assert.Equal(DeliveryStatus.Pending, savedDelivery.Status);
    }

    [Fact]
    public async Task AddDelivery_InvalidChildId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);
        var invalidChildId = Guid.NewGuid();
        var createDeliveryDto = new CreateDeliveryDto(invalidChildId, route.Id);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AddDeliveryAsync(createDeliveryDto));
    }

    [Fact]
    public async Task AddDelivery_InvalidRouteId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        santaDb.Children.Add(child);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);
        var invalidRouteId = Guid.NewGuid();
        var createDeliveryDto = new CreateDeliveryDto(child.Id, invalidRouteId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AddDeliveryAsync(createDeliveryDto));
    }

    [Fact]
    public async Task DeleteDelivery_ValidId_RemovesFromDatabase()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        var route = new Route { Name = "North Pole Route", Region = "Arctic", CapacityPerNight = 50 };
        santaDb.Children.Add(child);
        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        var delivery = new Delivery { ChildId = child.Id, RouteId = route.Id, Status = DeliveryStatus.Pending };
        santaDb.Deliveries.Add(delivery);
        await santaDb.SaveChangesAsync();

        var service = new DeliveriesService(santaDb);

        // Act
        await service.DeleteDeliveryAsync(delivery.Id);

        // Assert
        var deletedDelivery = await santaDb.Deliveries.FindAsync(delivery.Id);
        Assert.Null(deletedDelivery);
    }

    [Fact]
    public async Task DeleteDelivery_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new DeliveriesService(santaDb);
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteDeliveryAsync(invalidId));
    }
}