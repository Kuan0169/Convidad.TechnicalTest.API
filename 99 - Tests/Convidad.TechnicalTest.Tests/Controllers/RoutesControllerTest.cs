using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers;

public class RoutesControllerTest
{
    [Fact]
    public async Task GetAllRoutes_ReturnsAllRoutes()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routes = new List<RouteDto>
        {
            new RouteDto(Guid.NewGuid(), "North Pole Route", "Arctic", 50),
            new RouteDto(Guid.NewGuid(), "Europe Route", "Europe", 30)
        };
        mockService.Setup(s => s.GetAllRoutesAsync()).ReturnsAsync(routes);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.GetAllRoutes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoutes = Assert.IsAssignableFrom<IEnumerable<RouteDto>>(okResult.Value);
        Assert.Equal(2, returnedRoutes.Count());
    }

    [Fact]
    public async Task AddRoute_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var createRouteDto = new CreateRouteDto("Asia Route", "Asia", 40);
        var createdRoute = new RouteDto(Guid.NewGuid(), "Asia Route", "Asia", 40);
        mockService.Setup(s => s.AddRouteAsync(It.IsAny<CreateRouteDto>())).ReturnsAsync(createdRoute);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.AddRoute(createRouteDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedRoute = Assert.IsType<RouteDto>(createdResult.Value);
        Assert.Equal("Asia Route", returnedRoute.Name);
        Assert.Equal("Asia", returnedRoute.Region);
        Assert.Equal(40, returnedRoute.CapacityPerNight);
    }

    [Fact]
    public async Task AddRoute_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var invalidRouteDto = new CreateRouteDto("", "", -1);
        var controller = new RouteController(mockService.Object);
        controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await controller.AddRoute(invalidRouteDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        mockService.Verify(s => s.AddRouteAsync(It.IsAny<CreateRouteDto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoute_ValidId_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteRouteAsync(id)).Returns(Task.CompletedTask);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.DeleteRoute(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteRouteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteRoute_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteRouteAsync(id))
                  .ThrowsAsync(new KeyNotFoundException($"Route with ID {id} not found."));
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.DeleteRoute(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task AssignReindeerToRoute_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var reindeerId = Guid.NewGuid();
        var request = new AssignReindeerToRouteRequest(reindeerId, 10);
        mockService.Setup(s => s.AssignReindeerToRouteAsync(routeId, reindeerId, 10))
                  .Returns(Task.CompletedTask);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.AssignReindeerToRoute(routeId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.AssignReindeerToRouteAsync(routeId, reindeerId, 10), Times.Once);
    }

    [Fact]
    public async Task AssignReindeerToRoute_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var request = new AssignReindeerToRouteRequest(Guid.Empty, 10);
        var controller = new RouteController(mockService.Object);
        controller.ModelState.AddModelError("ReindeerId", "Invalid GUID");

        // Act
        var result = await controller.AssignReindeerToRoute(routeId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        mockService.Verify(s => s.AssignReindeerToRouteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AssignReindeerToRoute_RouteNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var reindeerId = Guid.NewGuid();
        var request = new AssignReindeerToRouteRequest(reindeerId, 10);
        mockService.Setup(s => s.AssignReindeerToRouteAsync(routeId, reindeerId, 10))
                  .ThrowsAsync(new KeyNotFoundException($"Route with ID {routeId} not found."));
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.AssignReindeerToRoute(routeId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(routeId.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task RemoveReindeerFromRoute_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var reindeerId = Guid.NewGuid();
        mockService.Setup(s => s.RemoveReindeerFromRouteAsync(routeId, reindeerId))
                  .Returns(Task.CompletedTask);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.RemoveReindeerFromRoute(routeId, reindeerId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.RemoveReindeerFromRouteAsync(routeId, reindeerId), Times.Once);
    }

    [Fact]
    public async Task RemoveReindeerFromRoute_NotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var reindeerId = Guid.NewGuid();
        mockService.Setup(s => s.RemoveReindeerFromRouteAsync(routeId, reindeerId))
                  .ThrowsAsync(new KeyNotFoundException($"Route or Reindeer not found."));
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.RemoveReindeerFromRoute(routeId, reindeerId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task GetReindeersForRoute_ValidRoute_ReturnsReindeers()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        var reindeers = new List<ReindeerDto>
        {
            new ReindeerDto(Guid.NewGuid(), "Rudolph", "XMAS-001", 100.0, 50),
            new ReindeerDto(Guid.NewGuid(), "Blitzen", "XMAS-002", 95.0, 45)
        };
        mockService.Setup(s => s.GetReindeersForRouteAsync(routeId)).ReturnsAsync(reindeers);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.GetReindeersForRoute(routeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReindeers = Assert.IsAssignableFrom<IEnumerable<ReindeerDto>>(okResult.Value);
        Assert.Equal(2, returnedReindeers.Count());
    }

    [Fact]
    public async Task GetReindeersForRoute_NotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        mockService.Setup(s => s.GetReindeersForRouteAsync(routeId))
                  .ThrowsAsync(new KeyNotFoundException($"Route with ID {routeId} not found."));
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.GetReindeersForRoute(routeId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(routeId.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task CanHandleNewDelivery_ValidRoute_ReturnsBoolean()
    {
        // Arrange
        var mockService = new Mock<IRoutesService>();
        var routeId = Guid.NewGuid();
        mockService.Setup(s => s.CanHandleNewDeliveryAsync(routeId)).ReturnsAsync(true);
        var controller = new RouteController(mockService.Object);

        // Act
        var result = await controller.CanHandleNewDelivery(routeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var canHandle = Assert.IsType<bool>(okResult.Value);
        Assert.True(canHandle);
    }
}