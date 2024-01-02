namespace ShroomCity.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    // POST api/account/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterInputModel model)
    {
        throw new NotImplementedException();
    }

    // POST api/account/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginInputModel model)
    {
        throw new NotImplementedException();
    }

    // GET api/account/logout
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    // GET api/account/profile
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        throw new NotImplementedException();
    }
}
