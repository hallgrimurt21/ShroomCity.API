namespace ShroomCity.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResearchersController : ControllerBase
{
    // GET /api/researchers
    [HttpGet]
    [Authorize(Policy = "read:researchers")]
    public async Task<IActionResult> GetResearchers()
    {
        throw new NotImplementedException();
    }

    // POST /api/researchers
    [HttpPost]
    [Authorize(Policy = "write:researchers")]
    public async Task<IActionResult> CreateResearcher([FromBody] ResearcherInputModel researcherInputModel)
    {
        throw new NotImplementedException();
    }

    // GET /api/researchers/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "read:researchers")]
    public async Task<IActionResult> GetResearcher(int id)
    {
        throw new NotImplementedException();
    }

    // GET /api/researchers/self
    [HttpGet("self")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> GetSelfResearcher()
    {
        throw new NotImplementedException();
    }
}
