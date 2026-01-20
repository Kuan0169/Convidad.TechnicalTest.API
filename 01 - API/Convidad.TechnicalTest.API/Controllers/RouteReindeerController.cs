using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RouteReindeerController(IRouteReindeerService routeReindeerService) : ControllerBase
    {
        private readonly IRouteReindeerService _routeReindeerService = routeReindeerService;

        [HttpPost("{routeId}/assign-reindeer")]
        public async Task<ActionResult> AssignReindeerToRoute(
            Guid routeId,
            [FromBody] AssignReindeerToRouteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _routeReindeerService.AssignReindeerToRouteAsync(
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
            await _routeReindeerService.RemoveReindeerFromRouteAsync(routeId, reindeerId);
            return NoContent();
        }

        [HttpGet("{routeId}/reindeers")]
        public async Task<ActionResult<IEnumerable<ReindeerDto>>> GetReindeersForRoute(Guid routeId)
        {
            var reindeers = await _routeReindeerService.GetReindeersForRouteAsync(routeId);
            return Ok(reindeers);
        }

        [HttpGet("{routeId}/can-handle-delivery")]
        public async Task<ActionResult<bool>> CanHandleNewDelivery(Guid routeId)
        {
            var canHandle = await _routeReindeerService.CanHandleNewDeliveryAsync(routeId);
            return Ok(canHandle);
        }
    }

    public record AssignReindeerToRouteRequest(
        Guid ReindeerId,
        int MaxDeliveries = 10
    );
}
