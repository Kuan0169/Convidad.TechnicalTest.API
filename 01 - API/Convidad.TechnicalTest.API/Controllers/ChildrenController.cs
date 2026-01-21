using Microsoft.AspNetCore.Mvc;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/children")]
public class ChildrenController(IChildrenService childrenService) : ControllerBase
{
    private readonly IChildrenService _childrenService = childrenService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChildDto>>> GetAllChildren()
    {
        var children = await _childrenService.GetAllChildrenAsync();
        return Ok(children);
    }

    [HttpGet("naughty")]
    public async Task<ActionResult<IEnumerable<ChildDto>>> GetNaughtyChildren()
    {
        var children = await _childrenService.GetNaughtyChildrenAsync();
        return Ok(children);
    }

    [HttpPost]
    public async Task<ActionResult<ChildDto>> AddChild([FromBody] CreateChildDto childDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDto = await _childrenService.AddChildAsync(childDto);
        return CreatedAtAction(nameof(GetAllChildren), new { id = createdDto.Id }, createdDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteChild(Guid id)
    {
        try
        {
            await _childrenService.DeleteChildAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
