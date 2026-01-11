using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Data.DTOs.Requests;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;


namespace Convidad.TechnicalTest.Tests.Controllers;

public class DeliveryControllerTest
{
    [Fact]
    public void AssignReindeerToDelivery_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<ISantaService>();
        var controller = new DeliveryController(mockService.Object);
        var deliveryId = Guid.NewGuid();
        var reindeerId = Guid.NewGuid();
        var request = new AssignReindeerRequest { ReindeerId = reindeerId };

        // Act
        var result = controller.AssignReindeerToDelivery(deliveryId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.AssignReindeerToDelivery(deliveryId, reindeerId), Times.Once);
    }

    [Fact]
    public void AssignReindeerToDelivery_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<ISantaService>();
        var controller = new DeliveryController(mockService.Object);

        controller.ModelState.AddModelError("ReindeerId", "Required");

        var deliveryId = Guid.NewGuid();
        var request = new AssignReindeerRequest { ReindeerId = Guid.Empty };

        // Act
        var result = controller.AssignReindeerToDelivery(deliveryId, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        mockService.Verify(s => s.AssignReindeerToDelivery(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetReindeerById_NotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<ISantaService>();
        var id = Guid.NewGuid(); // 👈 先建立具體 ID

        // 👈 使用具體 ID 設定 mock
        mockService
            .Setup(s => s.GetReindeerById(id))
            .Throws(new KeyNotFoundException($"Reindeer with ID {id} not found."));

        var controller = new DeliveryController(mockService.Object);

        // Act
        var result = controller.GetReindeerById(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var actualMessage = notFoundResult.Value?.ToString();

        Assert.NotNull(actualMessage);
        Assert.Contains(id.ToString(), actualMessage); // 👈 現在會匹配
        Assert.Contains("Reindeer", actualMessage);
        Assert.Contains("not found", actualMessage);
    }
}