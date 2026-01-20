using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;

public interface IDeliveriesService
{
    Task<IEnumerable<DeliveryDto>> GetDeliveriesAsync();
    Task<IEnumerable<DeliveryDto>> GetFailureDeliveriesAsync();
}

public class DeliveriesService(SantaDbContext santaDb) : IDeliveriesService
{
    private readonly SantaDbContext santaDb = santaDb;

    public async Task<IEnumerable<DeliveryDto>> GetDeliveriesAsync()
    {
        var deliveries = await santaDb.Deliveries.ToListAsync();
        return deliveries.Select(d => new DeliveryDto(
             d.Id, 
             d.ChildId, 
             d.RouteId, 
             d.Status.ToString(), 
             d.CreatedAt));
    }

    public async Task<IEnumerable<DeliveryDto>> GetFailureDeliveriesAsync()
    {
        var deliveries = await santaDb.Deliveries
            .Where(d => d.Status == DeliveryStatus.Failed)
            .ToListAsync();
        return deliveries.Select(d => new DeliveryDto(
            d.Id, 
            d.ChildId, 
            d.RouteId, 
            d.Status.ToString(),
            d.CreatedAt));
    }
}
