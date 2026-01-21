using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers;

public class ChildrenControllerTest
{
    [Fact]
    public async Task GetAllChildren_ReturnsAllChildren()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var children = new List<ChildDto>
        {
            new ChildDto(Guid.NewGuid(), "Alice", "US", true),
            new ChildDto(Guid.NewGuid(), "Bob", "CA", false)
        };

        mockService.Setup(s => s.GetChildrenAsync(null)).ReturnsAsync(children);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.GetAllChildren();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDto>>(okResult.Value);
        Assert.Equal(2, returnedChildren.Count());
    }

    [Fact]
    public async Task GetNaughtyChildren_ReturnsOnlyNaughtyChildren()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var naughtyChildren = new List<ChildDto>
        {
            new ChildDto(Guid.NewGuid(), "Charlie", "UK", false),
            new ChildDto(Guid.NewGuid(), "Diana", "AU", false)
        };

        mockService.Setup(s => s.GetChildrenAsync(false)).ReturnsAsync(naughtyChildren);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.GetNaughtyChildren();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDto>>(okResult.Value);
        Assert.Equal(2, returnedChildren.Count());

        // Verify all returned children are naughty (IsNice = false)
        Assert.All(returnedChildren, child => Assert.False(child.IsNice));
    }

    [Fact]
    public async Task GetNaughtyChildren_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var emptyList = new List<ChildDto>();

        mockService.Setup(s => s.GetChildrenAsync(false)).ReturnsAsync(emptyList);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.GetNaughtyChildren();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDto>>(okResult.Value);
        Assert.Empty(returnedChildren);
    }

    [Fact]
    public async Task AddChild_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var createChildDto = new CreateChildDto("Alice", "US", true);
        var createdChild = new ChildDto(Guid.NewGuid(), "Alice", "US", true);
        mockService.Setup(s => s.AddChildAsync(It.IsAny<CreateChildDto>())).ReturnsAsync(createdChild);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.AddChild(createChildDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedChild = Assert.IsType<ChildDto>(createdResult.Value);
        Assert.Equal("Alice", returnedChild.Name);
        Assert.Equal("US", returnedChild.CountryCode);
        Assert.True(returnedChild.IsNice);
    }

    [Fact]
    public async Task AddChild_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var invalidChildDto = new CreateChildDto("", "", true);
        var controller = new ChildrenController(mockService.Object);
        controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await controller.AddChild(invalidChildDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        mockService.Verify(s => s.AddChildAsync(It.IsAny<CreateChildDto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteChild_ValidId_ReturnsNoContent()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteChildAsync(id)).Returns(Task.CompletedTask);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.DeleteChild(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockService.Verify(s => s.DeleteChildAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteChild_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<IChildrenService>();
        var id = Guid.NewGuid();
        mockService.Setup(s => s.DeleteChildAsync(id))
                  .ThrowsAsync(new KeyNotFoundException($"Child with ID {id} not found."));
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.DeleteChild(id);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFoundResult.Value?.ToString());
    }
}