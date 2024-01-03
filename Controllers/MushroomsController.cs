namespace ShroomCity.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

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
        throw new NotImplementedException();
    }

    // POST api/mushrooms
    [HttpPost]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> CreateMushroom([FromBody] MushroomInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    // PUT api/mushrooms/{id}
    [HttpPut("{id}")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> UpdateMushroom(int id, bool performLookup, [FromBody] MushroomUpdateInputModel inputModel)
    {
        throw new NotImplementedException();
    }

    // DELETE api/mushrooms/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> DeleteMushroom(int id)
    {
        throw new NotImplementedException();
    }

    // POST api/mushrooms/{id}/research-entries
    [HttpPost("{id}/research-entries")]
    [Authorize(Policy = "write:mushrooms")]
    public async Task<IActionResult> CreateResearchEntry(int id, [FromBody] ResearchEntryInputModel model)
    {
        throw new NotImplementedException();
    }
}
