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

    [HttpPost]
    public async Task<ActionResult<RouteDto>> AddRoute([FromBody] CreateRouteDto routeDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDto = await _routesService.AddRouteAsync(routeDto);
        return CreatedAtAction(nameof(GetAllRoutes), new { id = createdDto.Id }, createdDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoute(Guid id)
    {
        try
        {
            await _routesService.DeleteRouteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}