using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers;

public class DeliveriesControllerTest
{
    [Fact]
    public async Task GetDeliveries_ReturnsAllDeliveries()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var deliveries = new List<DeliveryDto>
        {
            new DeliveryDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Pending", DateTimeOffset.UtcNow),
            new DeliveryDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Delivered", DateTimeOffset.UtcNow)
        };
        mockService.Setup(s => s.GetDeliveriesAsync()).ReturnsAsync(deliveries);
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.GetDeliveries();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDeliveries = Assert.IsAssignableFrom<IEnumerable<DeliveryDto>>(okResult.Value);
        Assert.Equal(2, returnedDeliveries.Count());
    }

    [Fact]
    public async Task GetFailureDeliveries_ReturnsOnlyFailedDeliveries()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var failedDeliveries = new List<DeliveryDto>
        {
            new DeliveryDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Failed", DateTimeOffset.UtcNow)
        };
        mockService.Setup(s => s.GetFailureDeliveriesAsync()).ReturnsAsync(failedDeliveries);
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.GetFailureDeliveries();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDeliveries = Assert.IsAssignableFrom<IEnumerable<DeliveryDto>>(okResult.Value);
        Assert.Single(returnedDeliveries);
        Assert.Equal("Failed", returnedDeliveries.First().Status);
    }
}