using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;

public interface IRoutesService
{
    Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
    Task<RouteDto> AddRouteAsync(CreateRouteDto routeDto);
    Task DeleteRouteAsync(Guid id);
}

public class RoutesService(SantaDbContext santaDb) : IRoutesService
{
    private readonly SantaDbContext santaDb = santaDb;

    public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
    {
        var routes = await santaDb.Routes.ToListAsync();
        return routes.Select(r => new RouteDto(r.Id, r.Name, r.Region, r.CapacityPerNight));
    }

    public async Task<RouteDto> AddRouteAsync(CreateRouteDto routeDto)
    {
        var route = new Route
        {
            Name = routeDto.Name,
            Region = routeDto.Region,
            CapacityPerNight = routeDto.CapacityPerNight
        };

        santaDb.Routes.Add(route);
        await santaDb.SaveChangesAsync();

        return new RouteDto(route.Id, route.Name, route.Region, route.CapacityPerNight);
    }

    public async Task DeleteRouteAsync(Guid id)
    {
        var route = await santaDb.Routes.FindAsync(id);
        if (route == null)
            throw new KeyNotFoundException($"Route with ID {id} not found.");

        santaDb.Routes.Remove(route);
        await santaDb.SaveChangesAsync();
    }
}
