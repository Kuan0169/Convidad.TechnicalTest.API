using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
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
    public async Task GetAllChildren_ReturnsAllChildren()
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
        var result = await service.GetAllChildrenAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetNaughtyChildren_ReturnsOnlyNaughtyChildren()
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
        var result = await service.GetNaughtyChildrenAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, child => Assert.False(child.IsNice));
    }

    [Fact]
    public async Task GetNaughtyChildren_EmptyList_ReturnsEmpty()
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
        var result = await service.GetNaughtyChildrenAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}