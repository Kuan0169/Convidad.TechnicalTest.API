using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Data.Enums;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;
public interface IDeliveriesService
{
    Task<IEnumerable<DeliveryDto>> GetDeliveriesAsync();
    Task<IEnumerable<DeliveryDto>> GetFailureDeliveriesAsync();
    Task<DeliveryDto> AddDeliveryAsync(CreateDeliveryDto deliveryDto);
    Task DeleteDeliveryAsync(Guid id);
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

    public async Task<DeliveryDto> AddDeliveryAsync(CreateDeliveryDto deliveryDto)
    {
        var childExists = await santaDb.Children.AnyAsync(c => c.Id == deliveryDto.ChildId);
        if (!childExists)
            throw new KeyNotFoundException($"Child with ID {deliveryDto.ChildId} not found.");

        var routeExists = await santaDb.Routes.AnyAsync(r => r.Id == deliveryDto.RouteId);
        if (!routeExists)
            throw new KeyNotFoundException($"Route with ID {deliveryDto.RouteId} not found.");

        var delivery = new Delivery
        {
            ChildId = deliveryDto.ChildId,
            RouteId = deliveryDto.RouteId,
            Status = DeliveryStatus.Pending,
            ScheduledFor = DateTime.UtcNow.AddDays(1) 
        };

        santaDb.Deliveries.Add(delivery);
        await santaDb.SaveChangesAsync();

        return new DeliveryDto(
        delivery.Id,
        delivery.ChildId,
        delivery.RouteId,
        delivery.Status.ToString(),
        delivery.CreatedAt);
    }

    public async Task DeleteDeliveryAsync(Guid id)
    {
        var delivery = await santaDb.Deliveries.FindAsync(id);
        if (delivery == null)
            throw new KeyNotFoundException($"Delivery with ID {id} not found.");

        santaDb.Deliveries.Remove(delivery);
        await santaDb.SaveChangesAsync();
    }
}
