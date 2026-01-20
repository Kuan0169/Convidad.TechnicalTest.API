using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services
{
    public interface IRouteReindeerService
    {
        Task AssignReindeerToRouteAsync(Guid routeId, Guid reindeerId, int maxDeliveries = 10);

        Task RemoveReindeerFromRouteAsync(Guid routeId, Guid reindeerId);

        Task<IEnumerable<ReindeerDto>> GetReindeersForRouteAsync(Guid routeId);

        Task<bool> CanHandleNewDeliveryAsync(Guid routeId);
    }

    public class RouteReindeerService(SantaDbContext dbContext) : IRouteReindeerService
    {
        private readonly SantaDbContext _dbContext = dbContext;

        public async Task AssignReindeerToRouteAsync(Guid routeId, Guid reindeerId, int maxDeliveries = 10)
        {
            var route = await _dbContext.Routes.FindAsync(routeId);
            if (route == null)
                throw new KeyNotFoundException($"Route with ID {routeId} not found.");

            var reindeer = await _dbContext.Reindeers.FindAsync(reindeerId);
            if (reindeer == null)
                throw new KeyNotFoundException($"Reindeer with ID {reindeerId} not found.");

            var existingAssignment = await _dbContext.RouteReindeers
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
                _dbContext.RouteReindeers.Add(assignment);
            }
            else
            {
                existingAssignment.MaxDeliveries = maxDeliveries;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveReindeerFromRouteAsync(Guid routeId, Guid reindeerId)
        {
            var assignment = await _dbContext.RouteReindeers
                .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.ReindeerId == reindeerId);

            if (assignment != null)
            {
                _dbContext.RouteReindeers.Remove(assignment);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReindeerDto>> GetReindeersForRouteAsync(Guid routeId)
        {
            var reindeers = await _dbContext.RouteReindeers
                .Where(rr => rr.RouteId == routeId)
                .Include(rr => rr.Reindeer)
                .Select(rr => rr.Reindeer)
                .ToListAsync();

            return reindeers.Select(r => new ReindeerDto(r.Id, r.Name, r.PlateNumber, r.Weight, r.Packets));
        }

        public async Task<bool> CanHandleNewDeliveryAsync(Guid routeId)
        {
            var assignments = await _dbContext.RouteReindeers
                .Where(rr => rr.RouteId == routeId)
                .ToListAsync();

            return assignments.Any(rr => rr.CurrentDeliveries < rr.MaxDeliveries);
        }
    }
}
