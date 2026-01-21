using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/reindeers")]
public class ReindeersController(IReindeersService reindeersService) : ControllerBase
{
    private readonly IReindeersService _reindeersService = reindeersService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReindeerDto>>> GetAllReindeers()
    {
        var reindeers = await _reindeersService.GetAllReindeersAsync();
        return Ok(reindeers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReindeerDto>> GetReindeerById(Guid id)
    {
        try
        {
            var reindeer = await _reindeersService.GetReindeerByIdAsync(id);
            return Ok(reindeer);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ReindeerDto>> AddReindeer([FromBody] ReindeerDto reindeerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDto = await _reindeersService.AddReindeerAsync(reindeerDto);
        return CreatedAtAction(nameof(GetReindeerById), new { id = createdDto.Id }, createdDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReindeer(Guid id)
    {
        try
        {
            await _reindeersService.DeleteReindeerAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
