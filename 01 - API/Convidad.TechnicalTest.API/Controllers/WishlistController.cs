using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/wishlist")]
public class WishlistController(IWishlistService wishlistService) : ControllerBase
{
    private readonly IWishlistService _wishlistService = wishlistService;

    [HttpGet("children/{childId}")]
    public async Task<ActionResult<IEnumerable<WishDto>>> GetWishlistByChildId(Guid childId)
    {
        var wishes = await _wishlistService.GetWishlistByChildIdAsync(childId);
        return Ok(wishes);
    }

    [HttpGet("children/{childId}/priority")]
    public async Task<ActionResult<IEnumerable<WishDto>>> GetWishlistByChildIdOrderedByPriority(Guid childId)
    {
        var wishes = await _wishlistService.GetWishlistByChildIdOrderedByPriorityAsync(childId);
        return Ok(wishes);
    }
}
