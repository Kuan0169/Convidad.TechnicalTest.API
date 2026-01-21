using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;
public interface IChildrenService
{
    Task<IEnumerable<ChildDto>> GetAllChildrenAsync();
    Task<IEnumerable<ChildDto>> GetNaughtyChildrenAsync();
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
}
