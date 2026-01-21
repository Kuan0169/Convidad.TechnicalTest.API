using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers
{
    public class RouteReindeerControllerTest
    {
        [Fact]
        public async Task AssignReindeerToRoute_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            var reindeerId = Guid.NewGuid();
            var request = new AssignReindeerToRouteRequest(reindeerId, 10);
            var controller = new RouteReindeerController(mockService.Object);

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
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            var request = new AssignReindeerToRouteRequest(Guid.Empty, 10); // Invalid ReindeerId
            var controller = new RouteReindeerController(mockService.Object);
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
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            var reindeerId = Guid.NewGuid();
            var request = new AssignReindeerToRouteRequest(reindeerId, 10);
            mockService.Setup(s => s.AssignReindeerToRouteAsync(routeId, reindeerId, 10))
                      .ThrowsAsync(new KeyNotFoundException($"Route with ID {routeId} not found."));
            var controller = new RouteReindeerController(mockService.Object);

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
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            var reindeerId = Guid.NewGuid();
            var controller = new RouteReindeerController(mockService.Object);

            // Act
            var result = await controller.RemoveReindeerFromRoute(routeId, reindeerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.RemoveReindeerFromRouteAsync(routeId, reindeerId), Times.Once);
        }

        [Fact]
        public async Task GetReindeersForRoute_ValidRoute_ReturnsReindeers()
        {
            // Arrange
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            var reindeers = new List<ReindeerDto>
        {
            new ReindeerDto(Guid.NewGuid(), "Rudolph", "XMAS-001", 100.0, 50),
            new ReindeerDto(Guid.NewGuid(), "Blitzen", "XMAS-002", 95.0, 45)
        };
            mockService.Setup(s => s.GetReindeersForRouteAsync(routeId)).ReturnsAsync(reindeers);
            var controller = new RouteReindeerController(mockService.Object);

            // Act
            var result = await controller.GetReindeersForRoute(routeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReindeers = Assert.IsAssignableFrom<IEnumerable<ReindeerDto>>(okResult.Value);
            Assert.Equal(2, returnedReindeers.Count());
        }

        [Fact]
        public async Task CanHandleNewDelivery_ValidRoute_ReturnsBoolean()
        {
            // Arrange
            var mockService = new Mock<IRouteReindeerService>();
            var routeId = Guid.NewGuid();
            mockService.Setup(s => s.CanHandleNewDeliveryAsync(routeId)).ReturnsAsync(true);
            var controller = new RouteReindeerController(mockService.Object);

            // Act
            var result = await controller.CanHandleNewDelivery(routeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var canHandle = Assert.IsType<bool>(okResult.Value);
            Assert.True(canHandle);
        }
    }
}
