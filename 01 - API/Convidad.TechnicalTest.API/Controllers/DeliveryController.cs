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
}
