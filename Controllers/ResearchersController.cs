namespace ShroomCity.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResearchersController : ControllerBase
{
    private readonly IResearcherService researcherService;

    public ResearchersController(IResearcherService researcherService) =>
        this.researcherService = researcherService;

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
