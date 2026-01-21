using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;
public interface IChildrenService
{
    Task<IEnumerable<ChildDto>> GetAllChildrenAsync();
    Task<IEnumerable<ChildDto>> GetNaughtyChildrenAsync();
    Task<ChildDto> AddChildAsync(CreateChildDto childDto);
    Task DeleteChildAsync(Guid id);
}

public class ChildrenService(SantaDbContext santaDb) : IChildrenService
{
    private readonly SantaDbContext santaDb = santaDb;

    public async Task<IEnumerable<ChildDto>> GetAllChildrenAsync()
    {
        var children = await santaDb.Children.ToListAsync();
        return children.Select(c => new ChildDto(c.Id, c.Name, c.CountryCode, c.IsNice));
    }

    public async Task<IEnumerable<ChildDto>> GetNaughtyChildrenAsync()
    {
        var children = await santaDb.Children
            .Where(c => !c.IsNice)
            .ToListAsync();
        return children.Select(c => new ChildDto(c.Id, c.Name, c.CountryCode, c.IsNice));
    }

    public async Task<ChildDto> AddChildAsync(CreateChildDto childDto)
    {
        var child = new Child
        {
            Name = childDto.Name,
            CountryCode = childDto.CountryCode,
            IsNice = childDto.IsNice
        };

        santaDb.Children.Add(child);
        await santaDb.SaveChangesAsync();

        return new ChildDto(child.Id, child.Name, child.CountryCode, child.IsNice);
    }

    public async Task DeleteChildAsync(Guid id)
    {
        var child = await santaDb.Children.FindAsync(id);
        if (child == null)
            throw new KeyNotFoundException($"Child with ID {id} not found.");

        santaDb.Children.Remove(child);
        await santaDb.SaveChangesAsync();
    }
}
