namespace ShroomCity.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.Dtos;
using ShroomCity.Models;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MushroomsController : ControllerBase
{
    private readonly IMushroomService mushroomService;

    public MushroomsController(IMushroomService mushroomService) =>
        this.mushroomService = mushroomService;
    // GET api/mushrooms
    [HttpGet]
    [Authorize(Policy = "read:mushrooms")]
    public async Task<IActionResult> GetMushrooms(string? name, int? stemSizeMinimum, int? stemSizeMaximum, int? capSizeMinimum, int? capSizeMaximum, string? color, int pageSize = 25, int pageNumber = 1)
    {
        var envelope = await this.mushroomService.GetMushrooms(name, stemSizeMinimum, stemSizeMaximum, capSizeMinimum, capSizeMaximum, color, pageSize, pageNumber);

        return this.Ok(envelope);
    }

    // GET api/mushrooms/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "read:mushrooms")]
    public async Task<IActionResult> GetMushroom(int id)
    {
        var mushroom = await this.mushroomService.GetMushroomById(id);

        if (mushroom == null)
        {
            return this.NotFound("Mushroom not found.");
        }

        return this.Ok(mushroom);
    }

    // GET api/mushrooms/lookup
    [HttpGet("lookup")]
    [Authorize(Policy = "read:mushrooms")]
    public async Task<IActionResult> GetLookupMushrooms(int pageSize = 25, int pageNumber = 1)
    {
        var mushrooms = await this.mushroomService.GetLookupMushrooms(pageSize, pageNumber);

        if (mushrooms == null || !mushrooms.Items.Any())
        {
            return this.NotFound("No mushrooms found.");
        }

        return this.Ok(mushrooms);
    }

    // POST api/mushrooms
    [HttpPost]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> CreateMushroom([FromBody] MushroomInputModel inputModel)
    {
        var researcherEmailAddress = this.User.FindFirst(ClaimTypes.Email)?.Value;
        if (researcherEmailAddress == null)
        {
            return this.Unauthorized("User is not authenticated.");
        }

        var newMushroomId = await this.mushroomService.CreateMushroom(researcherEmailAddress, inputModel);

        return this.CreatedAtAction(nameof(GetMushroom), new { id = newMushroomId }, inputModel);
    }

    // PUT api/mushrooms/{id}
    [HttpPut("{id}")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> UpdateMushroom(int id, bool performLookup, [FromBody] MushroomUpdateInputModel inputModel)
    {
        var result = await this.mushroomService.UpdateMushroomById(id, inputModel, performLookup);
        if (result)
        {
            return this.Ok("Mushroom updated");
        }
        else
        {
            return this.NotFound("Mushroom not found.");
        }
    }

    // DELETE api/mushrooms/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> DeleteMushroom(int id)
    {
        var result = await this.mushroomService.DeleteMushroomById(id);

        if (!result)
        {
            return this.NotFound("Mushroom not found.");
        }

        return this.Ok("Mushroom deleted.");
    }

    // POST api/mushrooms/{id}/research-entries
    [HttpPost("{id}/research-entries")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> CreateResearchEntry(int id, [FromBody] ResearchEntryInputModel model)
    {
        throw new NotImplementedException();
    }
}
