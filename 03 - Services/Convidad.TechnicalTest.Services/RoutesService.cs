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
    Task AssignReindeerToRouteAsync(Guid routeId, Guid reindeerId, int maxDeliveries = 10);
    Task RemoveReindeerFromRouteAsync(Guid routeId, Guid reindeerId);
    Task<IEnumerable<ReindeerDto>> GetReindeersForRouteAsync(Guid routeId);
    Task<bool> CanHandleNewDeliveryAsync(Guid routeId);
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

    public async Task AssignReindeerToRouteAsync(Guid routeId, Guid reindeerId, int maxDeliveries = 10)
    {
        var route = await santaDb.Routes.FindAsync(routeId);
        if (route == null)
            throw new KeyNotFoundException($"Route with ID {routeId} not found.");

        var reindeer = await santaDb.Reindeers.FindAsync(reindeerId);
        if (reindeer == null)
            throw new KeyNotFoundException($"Reindeer with ID {reindeerId} not found.");

        var existingAssignment = await santaDb.RouteReindeers
            .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.ReindeerId == reindeerId);

        if (existingAssignment == null)
        {
            var assignment = new RouteReindeer
            {
                RouteId = routeId,
                ReindeerId = reindeerId,
                MaxDeliveries = maxDeliveries,
                CurrentDeliveries = 0
            };
            santaDb.RouteReindeers.Add(assignment);
        }
        else
        {
            existingAssignment.MaxDeliveries = maxDeliveries;
        }

        await santaDb.SaveChangesAsync();
    }

    public async Task RemoveReindeerFromRouteAsync(Guid routeId, Guid reindeerId)
    {
        var assignment = await santaDb.RouteReindeers
            .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.ReindeerId == reindeerId);

        if (assignment != null)
        {
            santaDb.RouteReindeers.Remove(assignment);
            await santaDb.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ReindeerDto>> GetReindeersForRouteAsync(Guid routeId)
    {
        var route = await santaDb.Routes.FindAsync(routeId);
        if (route == null)
            throw new KeyNotFoundException($"Route with ID {routeId} not found.");

        var reindeers = await santaDb.RouteReindeers
            .Where(rr => rr.RouteId == routeId)
            .Include(rr => rr.Reindeer)
            .Select(rr => rr.Reindeer)
            .ToListAsync();

        return reindeers.Select(r => new ReindeerDto(r.Id, r.Name, r.PlateNumber, r.Weight, r.Packets));
    }

    public async Task<bool> CanHandleNewDeliveryAsync(Guid routeId)
    {
        var assignments = await santaDb.RouteReindeers
            .Where(rr => rr.RouteId == routeId)
            .ToListAsync();

        return assignments.Any(rr => rr.CurrentDeliveries < rr.MaxDeliveries);
    }
}
