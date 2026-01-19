using Microsoft.AspNetCore.Mvc;
using Convidad.TechnicalTest.Models.DTOs;
using Convidad.TechnicalTest.Services.SantaService;

namespace Convidad.TechnicalTest.API.Controllers;

[ApiController]
[Route("api/children")]
public class ChildrenController(IChildrenService childrenService) : ControllerBase
{
    private readonly IChildrenService _childrenService = childrenService;

    [HttpGet]
    public ActionResult<IEnumerable<ChildDto>> GetAllChildren()
    {
        return Ok(_childrenService.GetAllChildren());
    }

    [HttpGet("naughty")]
    public ActionResult<IEnumerable<ChildDto>> GetNaughtyChildren()
    {
        return Ok(_childrenService.GetNaughtyChildren());
    }
}
