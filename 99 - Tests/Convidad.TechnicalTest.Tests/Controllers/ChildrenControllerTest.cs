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
        mockService.Setup(s => s.GetAllChildrenAsync()).ReturnsAsync(children);
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
        mockService.Setup(s => s.GetNaughtyChildrenAsync()).ReturnsAsync(naughtyChildren);
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
        mockService.Setup(s => s.GetNaughtyChildrenAsync()).ReturnsAsync(emptyList);
        var controller = new ChildrenController(mockService.Object);

        // Act
        var result = await controller.GetNaughtyChildren();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDto>>(okResult.Value);
        Assert.Empty(returnedChildren);
    }
}