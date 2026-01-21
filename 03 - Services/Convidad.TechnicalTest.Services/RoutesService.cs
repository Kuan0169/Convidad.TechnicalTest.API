using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;

public interface IRoutesService
{
    Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
}

public class RoutesService(SantaDbContext santaDb) : IRoutesService
{
    private readonly SantaDbContext _santaDb = santaDb;

    public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
    {
        var routes = await _santaDb.Routes.ToListAsync();
        return routes.Select(r => new RouteDto(r.Id, r.Name, r.Region, r.CapacityPerNight));
    }
}
