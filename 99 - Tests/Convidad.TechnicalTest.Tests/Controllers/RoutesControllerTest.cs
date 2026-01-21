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
}