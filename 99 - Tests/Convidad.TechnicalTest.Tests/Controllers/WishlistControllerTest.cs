using Convidad.TechnicalTest.API.Controllers;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Controllers;

public class WishlistControllerTest
{
    [Fact]
    public async Task GetWishlistByChildId_ValidChildId_ReturnsWishes()
    {
        // Arrange
        var mockService = new Mock<IWishlistService>();
        var childId = Guid.NewGuid();
        var wishes = new List<WishDto>
        {
            new WishDto(Guid.NewGuid(), "Toys", 5),
            new WishDto(Guid.NewGuid(), "Books", 3)
        };
        mockService.Setup(s => s.GetWishlistByChildIdAsync(childId)).ReturnsAsync(wishes);
        var controller = new WishlistController(mockService.Object);

        // Act
        var result = await controller.GetWishlistByChildId(childId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWishes = Assert.IsAssignableFrom<IEnumerable<WishDto>>(okResult.Value);
        Assert.Equal(2, returnedWishes.Count());
    }

    [Fact]
    public async Task GetWishlistByChildId_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var mockService = new Mock<IWishlistService>();
        var childId = Guid.NewGuid();
        var emptyList = new List<WishDto>();
        mockService.Setup(s => s.GetWishlistByChildIdAsync(childId)).ReturnsAsync(emptyList);
        var controller = new WishlistController(mockService.Object);

        // Act
        var result = await controller.GetWishlistByChildId(childId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWishes = Assert.IsAssignableFrom<IEnumerable<WishDto>>(okResult.Value);
        Assert.Empty(returnedWishes);
    }

    [Fact]
    public async Task GetWishlistByChildIdOrderedByPriority_ValidChildId_ReturnsSortedWishes()
    {
        // Arrange
        var mockService = new Mock<IWishlistService>();
        var childId = Guid.NewGuid();
        var wishes = new List<WishDto>
        {
            new WishDto(Guid.NewGuid(), "Books", 3),
            new WishDto(Guid.NewGuid(), "Toys", 5),
            new WishDto(Guid.NewGuid(), "Clothes", 1)
        };
        // Service should return sorted by priority (descending)
        var sortedWishes = new List<WishDto>
        {
            new WishDto(Guid.NewGuid(), "Toys", 5),
            new WishDto(Guid.NewGuid(), "Books", 3),
            new WishDto(Guid.NewGuid(), "Clothes", 1)
        };
        mockService.Setup(s => s.GetWishlistByChildIdOrderedByPriorityAsync(childId)).ReturnsAsync(sortedWishes);
        var controller = new WishlistController(mockService.Object);

        // Act
        var result = await controller.GetWishlistByChildIdOrderedByPriority(childId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWishes = Assert.IsAssignableFrom<IEnumerable<WishDto>>(okResult.Value);
        Assert.Equal(3, returnedWishes.Count());

        // Verify descending order by priority
        var priorities = returnedWishes.Select(w => w.Priority).ToList();
        var expectedDescending = priorities.OrderByDescending(p => p).ToList();
        Assert.Equal(expectedDescending, priorities);
    }

    [Fact]
    public async Task GetWishlistByChildIdOrderedByPriority_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var mockService = new Mock<IWishlistService>();
        var childId = Guid.NewGuid();
        var emptyList = new List<WishDto>();
        mockService.Setup(s => s.GetWishlistByChildIdOrderedByPriorityAsync(childId)).ReturnsAsync(emptyList);
        var controller = new WishlistController(mockService.Object);

        // Act
        var result = await controller.GetWishlistByChildIdOrderedByPriority(childId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedWishes = Assert.IsAssignableFrom<IEnumerable<WishDto>>(okResult.Value);
        Assert.Empty(returnedWishes);
    }
}