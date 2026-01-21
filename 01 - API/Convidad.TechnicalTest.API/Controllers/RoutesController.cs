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

    [HttpPost("{routeId}/assign-reindeer")]
    public async Task<ActionResult> AssignReindeerToRoute(
        Guid routeId,
        [FromBody] AssignReindeerToRouteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _routesService.AssignReindeerToRouteAsync(
                routeId,
                request.ReindeerId,
                request.MaxDeliveries);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{routeId}/reindeers/{reindeerId}")]
    public async Task<ActionResult> RemoveReindeerFromRoute(Guid routeId, Guid reindeerId)
    {
        try
        {
            await _routesService.RemoveReindeerFromRouteAsync(routeId, reindeerId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{routeId}/reindeers")]
    public async Task<ActionResult<IEnumerable<ReindeerDto>>> GetReindeersForRoute(Guid routeId)
    {
        try
        {
            var reindeers = await _routesService.GetReindeersForRouteAsync(routeId);
            return Ok(reindeers);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{routeId}/can-handle-delivery")]
    public async Task<ActionResult<bool>> CanHandleNewDelivery(Guid routeId)
    {
        var canHandle = await _routesService.CanHandleNewDeliveryAsync(routeId);
        return Ok(canHandle);
    }
}