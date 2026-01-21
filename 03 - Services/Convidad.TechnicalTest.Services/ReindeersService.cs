using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;
public interface IReindeersService
{
    Task<IEnumerable<ReindeerDto>> GetAllReindeersAsync();
    Task<ReindeerDto> GetReindeerByIdAsync(Guid id);
    Task<ReindeerDto> AddReindeerAsync(ReindeerDto reindeerDto);
    Task DeleteReindeerAsync(Guid id);
}

public class ReindeersService(SantaDbContext santaDb) : IReindeersService
{
    private readonly SantaDbContext santaDb = santaDb;

    public async Task<IEnumerable<ReindeerDto>> GetAllReindeersAsync()
    {
        var reindeers = await santaDb.Reindeers.ToListAsync();
        return reindeers.Select(r => new ReindeerDto(r.Id, r.Name, r.PlateNumber, r.Weight, r.Packets));
    }

    public async Task<ReindeerDto> GetReindeerByIdAsync(Guid id)
    {
        var reindeer = await santaDb.Reindeers.FindAsync(id);
        if (reindeer == null)
            throw new KeyNotFoundException($"Reindeer with ID {id} not found.");

        return new ReindeerDto(reindeer.Id, reindeer.Name, reindeer.PlateNumber, reindeer.Weight, reindeer.Packets);
    }

    public async Task<ReindeerDto> AddReindeerAsync(ReindeerDto reindeerDto)
    {
        var reindeer = new Reindeer
        {
            Name = reindeerDto.Name,
            PlateNumber = reindeerDto.PlateNumber,
            Weight = reindeerDto.Weight,
            Packets = reindeerDto.Packets
        };

        santaDb.Reindeers.Add(reindeer);
        await santaDb.SaveChangesAsync();

        return new ReindeerDto(reindeer.Id, reindeer.Name, reindeer.PlateNumber, reindeer.Weight, reindeer.Packets);
    }

    public async Task DeleteReindeerAsync(Guid id)
    {
        var reindeer = await santaDb.Reindeers.FindAsync(id);
        if (reindeer == null)
            throw new KeyNotFoundException($"Reindeer with ID {id} not found.");

        santaDb.Reindeers.Remove(reindeer);
        await santaDb.SaveChangesAsync();
    }
}
