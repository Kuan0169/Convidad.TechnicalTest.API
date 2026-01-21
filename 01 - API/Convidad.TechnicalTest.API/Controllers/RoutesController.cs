using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController(IRoutesService routesService) : ControllerBase
{
    private readonly IRoutesService _routesService = routesService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
    {
        var routes = await _routesService.GetAllRoutesAsync();
        return Ok(routes);
    }
}