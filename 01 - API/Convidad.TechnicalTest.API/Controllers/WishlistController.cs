using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/wishlist")]
public class WishlistController(IWishlistService wishlistService) : ControllerBase
{
    private readonly IWishlistService _wishlistService = wishlistService;

    [HttpGet("children/{childId}")]
    public ActionResult<IEnumerable<WishDto>> GetWishlistByChildId(Guid childId)
    {
        return Ok(_wishlistService.GetWishlistByChildId(childId));
    }

    [HttpGet("children/{childId}/priority")]
    public ActionResult<IEnumerable<WishDto>> GetWishlistByChildIdOrderedByPriority(Guid childId)
    {
        return Ok(_wishlistService.GetWishlistByChildIdOrderedByPriority(childId));
    }
}
