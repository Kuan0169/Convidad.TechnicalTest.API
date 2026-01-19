using Convidad.TechnicalTest.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/reindeers")]
public class ReindeersController(IReindeersService reindeersService) : ControllerBase
{
    private readonly IReindeersService _reindeersService = reindeersService;

    [HttpGet]
    public ActionResult<IEnumerable<ReindeerDto>> GetAllReindeers()
    {
        return Ok(_reindeersService.GetAllReindeers());
    }

    [HttpGet("{id}")]
    public ActionResult<ReindeerDto> GetReindeerById(Guid id)
    {
        try
        {
            return Ok(_reindeersService.GetReindeerById(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public ActionResult<ReindeerDto> AddReindeer([FromBody] ReindeerDto reindeerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDto = _reindeersService.AddReindeer(reindeerDto);
        return CreatedAtAction(nameof(GetReindeerById), new { id = createdDto.Id }, createdDto);
    }
}
