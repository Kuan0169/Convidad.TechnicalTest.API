using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class WishlistServiceTest
{
    protected readonly SantaDbContext santaDb;

    public WishlistServiceTest()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<SantaDbContext>()
            .UseSqlite(connection)
            .Options;
        santaDb = new SantaDbContext(options);
        santaDb.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetWishlistByChildId_ValidChildId_ReturnsWishes()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        santaDb.Children.Add(child);
        await santaDb.SaveChangesAsync();

        var wishes = new List<Wish>
        {
            new Wish { ChildId = child.Id, Category = WishCategory.Other, Priority = 5 },
            new Wish { ChildId = child.Id, Category = WishCategory.Other, Priority = 3 }
        };
        santaDb.Wishes.AddRange(wishes);
        await santaDb.SaveChangesAsync();

        var service = new WishlistService(santaDb);

        // Act
        var result = await service.GetWishlistByChildIdAsync(child.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetWishlistByChildId_InvalidChildId_ReturnsEmpty()
    {
        // Arrange
        var service = new WishlistService(santaDb);
        var nonExistentChildId = Guid.NewGuid();

        // Act
        var result = await service.GetWishlistByChildIdAsync(nonExistentChildId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWishlistByChildIdOrderedByPriority_ValidChildId_ReturnsSortedWishes()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        santaDb.Children.Add(child);
        await santaDb.SaveChangesAsync();

        var wishes = new List<Wish>
        {
            new Wish { ChildId = child.Id, Category = WishCategory.Other, Priority = 3 },
            new Wish { ChildId = child.Id, Category = WishCategory.Other, Priority = 5 },
            new Wish { ChildId = child.Id, Category = WishCategory.Clothes, Priority = 1 }
        };
        santaDb.Wishes.AddRange(wishes);
        await santaDb.SaveChangesAsync();

        var service = new WishlistService(santaDb);

        // Act
        var result = await service.GetWishlistByChildIdOrderedByPriorityAsync(child.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());

        // Verify descending order by priority
        var priorities = result.Select(w => w.Priority).ToList();
        var expectedDescending = priorities.OrderByDescending(p => p).ToList();
        Assert.Equal(expectedDescending, priorities);
    }

    [Fact]
    public async Task GetWishlistByChildIdOrderedByPriority_InvalidChildId_ReturnsEmpty()
    {
        // Arrange
        var service = new WishlistService(santaDb);
        var nonExistentChildId = Guid.NewGuid();

        // Act
        var result = await service.GetWishlistByChildIdOrderedByPriorityAsync(nonExistentChildId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}