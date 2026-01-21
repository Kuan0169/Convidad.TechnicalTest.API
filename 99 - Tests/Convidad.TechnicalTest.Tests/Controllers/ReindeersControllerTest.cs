using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers;

public class ReindeersControllerTest
{
    [Fact]
    public async Task GetAllReindeers_ReturnsAllReindeers()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var reindeers = new List<ReindeerDto>
        {
            new ReindeerDto(Guid.NewGuid(), "Rudolph", "XMAS-001", 100.0, 50),
            new ReindeerDto(Guid.NewGuid(), "Blitzen", "XMAS-002", 95.0, 45)
        };
        mockService.Setup(s => s.GetAllReindeersAsync()).ReturnsAsync(reindeers);
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.GetAllReindeers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReindeers = Assert.IsAssignableFrom<IEnumerable<ReindeerDto>>(okResult.Value);
        Assert.Equal(2, returnedReindeers.Count());
    }

    [Fact]
    public async Task GetReindeerById_ValidId_ReturnsReindeer()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var id = Guid.NewGuid();
        var reindeer = new ReindeerDto(id, "Rudolph", "XMAS-001", 100.0, 50);
        mockService.Setup(s => s.GetReindeerByIdAsync(id)).ReturnsAsync(reindeer);
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.GetReindeerById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedReindeer = Assert.IsType<ReindeerDto>(okResult.Value);
        Assert.Equal(id, returnedReindeer.Id);
        Assert.Equal("Rudolph", returnedReindeer.Name);
    }

    [Fact]
    public async Task GetReindeerById_NotFound_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.GetReindeerByIdAsync(id))
                  .ThrowsAsync(new KeyNotFoundException($"Reindeer with ID {id} not found."));
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.GetReindeerById(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task AddReindeer_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var reindeerDto = new ReindeerDto(Guid.NewGuid(), "Comet", "XMAS-003", 90.0, 40);
        mockService.Setup(s => s.AddReindeerAsync(It.IsAny<ReindeerDto>())).ReturnsAsync(reindeerDto);
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.AddReindeer(reindeerDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedReindeer = Assert.IsType<ReindeerDto>(createdResult.Value);
        Assert.Equal(reindeerDto.Id, returnedReindeer.Id);
    }

    [Fact]
    public async Task AddReindeer_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var invalidReindeerDto = new ReindeerDto(Guid.Empty, "", "", -1, -1);
        var controller = new ReindeerController(mockService.Object);
        controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await controller.AddReindeer(invalidReindeerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        mockService.Verify(s => s.AddReindeerAsync(It.IsAny<ReindeerDto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteReindeer_ValidId_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteReindeerAsync(id)).Returns(Task.CompletedTask);
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.DeleteReindeer(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteReindeerAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteReindeer_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IReindeersService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteReindeerAsync(id))
                  .ThrowsAsync(new KeyNotFoundException($"Reindeer with ID {id} not found."));
        var controller = new ReindeerController(mockService.Object);

        // Act
        var result = await controller.DeleteReindeer(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }
}