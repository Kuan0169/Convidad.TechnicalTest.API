using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/deliveries")]
public class DeliveriesController(IDeliveriesService deliveriesService) : ControllerBase
{
    private readonly IDeliveriesService _deliveriesService = deliveriesService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetDeliveries()
    {
        var deliveries = await _deliveriesService.GetDeliveriesAsync();
        return Ok(deliveries);
    }

    [HttpGet("failures")]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetFailureDeliveries()
    {
        var deliveries = await _deliveriesService.GetFailureDeliveriesAsync();
        return Ok(deliveries);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryDto>> AddDelivery([FromBody] CreateDeliveryDto deliveryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdDto = await _deliveriesService.AddDeliveryAsync(deliveryDto);
            return CreatedAtAction(nameof(GetDeliveries), new { id = createdDto.Id }, createdDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDelivery(Guid id)
    {
        try
        {
            await _deliveriesService.DeleteDeliveryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
