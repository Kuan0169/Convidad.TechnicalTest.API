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
        var controller = new RoutesController(mockService.Object);

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
        var controller = new RoutesController(mockService.Object);

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
        var controller = new RoutesController(mockService.Object);
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
        var controller = new RoutesController(mockService.Object);

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
        var controller = new RoutesController(mockService.Object);

        // Act
        var result = await controller.DeleteRoute(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }
}