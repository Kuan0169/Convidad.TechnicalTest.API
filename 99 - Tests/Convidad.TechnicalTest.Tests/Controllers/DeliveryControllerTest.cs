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

    [Fact]
    public async Task AddDelivery_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var childId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var createDeliveryDto = new CreateDeliveryDto(childId, routeId);
        var createdDelivery = new DeliveryDto(Guid.NewGuid(), childId, routeId, "Pending", DateTimeOffset.UtcNow);
        mockService.Setup(s => s.AddDeliveryAsync(It.IsAny<CreateDeliveryDto>())).ReturnsAsync(createdDelivery);
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.AddDelivery(createDeliveryDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedDelivery = Assert.IsType<DeliveryDto>(createdResult.Value);
        Assert.Equal(childId, returnedDelivery.ChildId);
        Assert.Equal(routeId, returnedDelivery.RouteId);
        Assert.Equal("Pending", returnedDelivery.Status);
    }

    [Fact]
    public async Task AddDelivery_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var invalidDeliveryDto = new CreateDeliveryDto(Guid.Empty, Guid.Empty);
        var controller = new DeliveriesController(mockService.Object);
        controller.ModelState.AddModelError("ChildId", "Invalid GUID");

        // Act
        var result = await controller.AddDelivery(invalidDeliveryDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        mockService.Verify(s => s.AddDeliveryAsync(It.IsAny<CreateDeliveryDto>()), Times.Never);
    }

    [Fact]
    public async Task AddDelivery_ChildNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var childId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var createDeliveryDto = new CreateDeliveryDto(childId, routeId);
        mockService.Setup(s => s.AddDeliveryAsync(It.IsAny<CreateDeliveryDto>()))
                  .ThrowsAsync(new KeyNotFoundException($"Child with ID {childId} not found."));
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.AddDelivery(createDeliveryDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(childId.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task AddDelivery_RouteNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var childId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var createDeliveryDto = new CreateDeliveryDto(childId, routeId);
        mockService.Setup(s => s.AddDeliveryAsync(It.IsAny<CreateDeliveryDto>()))
                  .ThrowsAsync(new KeyNotFoundException($"Route with ID {routeId} not found."));
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.AddDelivery(createDeliveryDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(routeId.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task DeleteDelivery_ValidId_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteDeliveryAsync(id)).Returns(Task.CompletedTask);
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.DeleteDelivery(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteDeliveryAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteDelivery_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IDeliveriesService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteDeliveryAsync(id))
                  .ThrowsAsync(new KeyNotFoundException($"Delivery with ID {id} not found."));
        var controller = new DeliveriesController(mockService.Object);

        // Act
        var result = await controller.DeleteDelivery(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }
}