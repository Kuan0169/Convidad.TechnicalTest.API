using Convidad.TechnicalTest.Data.DTOs.Requests;
using Convidad.TechnicalTest.Data.Entities;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services.SantaService;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/deliveries")]
public class DeliveriesController(IDeliveriesService deliveriesService) : ControllerBase
{
    private readonly IDeliveriesService _deliveriesService = deliveriesService;

    [HttpGet]
    public ActionResult<IEnumerable<DeliveryDto>> GetDeliveries()
    {
        return Ok(_deliveriesService.GetDeliveries());
    }

    [HttpGet("failures")]
    public ActionResult<IEnumerable<DeliveryDto>> GetFailureDeliveries()
    {
        return Ok(_deliveriesService.GetFailureDeliveries());
    }

    [HttpPost("{deliveryId}/assign-reindeer")]
    public ActionResult AssignReindeerToDelivery(Guid deliveryId, [FromBody] AssignReindeerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _deliveriesService.AssignReindeerToDelivery(deliveryId, request.ReindeerId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
