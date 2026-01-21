using Convidad.TechnicalTest.Data.Context;
using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Convidad.TechnicalTest.Services;

public interface IWishlistService
{
    Task<IEnumerable<WishDto>> GetWishlistByChildIdAsync(Guid childId);
    Task<IEnumerable<WishDto>> GetWishlistByChildIdOrderedByPriorityAsync(Guid childId);
}

public class WishlistService(SantaDbContext santaDb) : IWishlistService
{
    private readonly SantaDbContext santaDb = santaDb;

    public async Task<IEnumerable<WishDto>> GetWishlistByChildIdAsync(Guid childId)
    {
        var wishes = await santaDb.Wishes
            .Where(w => w.ChildId == childId)
            .ToListAsync();
        return wishes.Select(w => new WishDto(w.Id, w.Category.ToString(), w.Priority));
    }

    public async Task<IEnumerable<WishDto>> GetWishlistByChildIdOrderedByPriorityAsync(Guid childId)
    {
        var wishes = await santaDb.Wishes
            .Where(w => w.ChildId == childId)
            .OrderByDescending(w => w.Priority)
            .ToListAsync();
        return wishes.Select(w => new WishDto(w.Id, w.Category.ToString(), w.Priority));
    }
}
