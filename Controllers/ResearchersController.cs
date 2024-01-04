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
        var researchers = await this.researcherService.GetAllResearchers();
        return this.Ok(researchers);
    }

    // POST /api/researchers
    [HttpPost]
    [Authorize(Policy = "write:researchers")]
    public async Task<IActionResult> CreateResearcher([FromBody] ResearcherInputModel researcherInputModel)
    {
        if (researcherInputModel == null)
        {
            return this.BadRequest("Researcher input data is null");
        }

        var createdBy = this.User.Identity?.Name;
        if (createdBy == null)
        {
            return this.Unauthorized("User identity does not exist");
        }

        var researcherId = await this.researcherService.CreateResearcher(createdBy, researcherInputModel);

        if (researcherId == null)
        {
            return this.BadRequest("Researcher could not be created");
        }

        return this.CreatedAtAction(nameof(GetResearcher), new { id = researcherId }, researcherInputModel);
    }

    // GET /api/researchers/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "read:researchers")]
    public async Task<IActionResult> GetResearcher(int id)
    {
        var researcher = await this.researcherService.GetResearcherById(id);
        if (researcher == null)
        {
            return this.NotFound("Researcher not found");
        }
        return this.Ok(researcher);
    }

    // GET /api/researchers/self
    [HttpGet("self")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> GetSelfResearcher()
    {
        throw new NotImplementedException();
    }
}
