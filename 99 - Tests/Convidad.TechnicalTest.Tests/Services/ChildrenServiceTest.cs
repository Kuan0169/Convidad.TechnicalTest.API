using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Convidad.TechnicalTest.Tests.Services;

public class ChildrenServiceTest
{
    protected readonly SantaDbContext santaDb;

    public ChildrenServiceTest()
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
    public async Task GetChildrenAsync_NullFilter_ReturnsAllChildren()
    {
        // Arrange
        var children = new List<Child>
        {
            new Child { Name = "Alice", CountryCode = "US", IsNice = true },
            new Child { Name = "Bob", CountryCode = "CA", IsNice = false }
        };
        santaDb.Children.AddRange(children);
        await santaDb.SaveChangesAsync();

        var service = new ChildrenService(santaDb);

        // Act
        var result = await service.GetChildrenAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetChildrenAsync_IsNiceFalse_ReturnsOnlyNaughtyChildren()
    {
        // Arrange
        var children = new List<Child>
        {
            new Child { Name = "Alice", CountryCode = "US", IsNice = true },
            new Child { Name = "Bob", CountryCode = "CA", IsNice = false },
            new Child { Name = "Charlie", CountryCode = "UK", IsNice = false }
        };
        santaDb.Children.AddRange(children);
        await santaDb.SaveChangesAsync();

        var service = new ChildrenService(santaDb);

        // Act
        var result = await service.GetChildrenAsync(false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, child => Assert.False(child.IsNice));
    }

    [Fact]
    public async Task GetChildrenAsync_IsNiceFalse_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var children = new List<Child>
        {
            new Child { Name = "Alice", CountryCode = "US", IsNice = true },
            new Child { Name = "Bob", CountryCode = "CA", IsNice = true }
        };
        santaDb.Children.AddRange(children);
        await santaDb.SaveChangesAsync();

        var service = new ChildrenService(santaDb);

        // Act
        var result = await service.GetChildrenAsync(false);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddChild_ValidChild_AddsToDatabase()
    {
        // Arrange
        var service = new ChildrenService(santaDb);
        var createChildDto = new CreateChildDto("Diana", "AU", false);

        // Act
        var result = await service.AddChildAsync(createChildDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Diana", result.Name);
        Assert.Equal("AU", result.CountryCode);
        Assert.False(result.IsNice);

        // Verify it was saved to database
        var savedChild = await santaDb.Children.FindAsync(result.Id);
        Assert.NotNull(savedChild);
        Assert.Equal("Diana", savedChild.Name);
    }

    [Fact]
    public async Task DeleteChild_ValidId_RemovesFromDatabase()
    {
        // Arrange
        var child = new Child { Name = "Alice", CountryCode = "US", IsNice = true };
        santaDb.Children.Add(child);
        await santaDb.SaveChangesAsync();

        var service = new ChildrenService(santaDb);

        // Act
        await service.DeleteChildAsync(child.Id);

        // Assert
        var deletedChild = await santaDb.Children.FindAsync(child.Id);
        Assert.Null(deletedChild);
    }

    [Fact]
    public async Task DeleteChild_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var service = new ChildrenService(santaDb);
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteChildAsync(invalidId));
    }
}