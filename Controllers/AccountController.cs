namespace ShroomCity.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using ShroomCity.Models.InputModels;
using ShroomCity.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly ITokenService tokenService;

    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        this.accountService = accountService;
        this.tokenService = tokenService;
    }

    // POST api/account/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterInputModel model)
    {
        var user = await this.accountService.Register(model);
        if (user == null)
        {
            return this.BadRequest("Registration failed");
        }

        var token = this.tokenService.GenerateJwtToken(user);
        return this.Ok(token);
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
