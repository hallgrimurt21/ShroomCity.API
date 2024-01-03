namespace ShroomCity.API.Controllers;

using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Register(RegisterInputModel inputModel)
    {
        var user = await this.accountService.Register(inputModel);
        if (user == null)
        {
            return this.BadRequest("Registration failed");
        }

        var token = this.tokenService.GenerateJwtToken(user);
        return this.Ok(token);
    }

    // POST api/account/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginInputModel inputModel)
    {
        var user = await this.accountService.SignIn(inputModel);
        if (user == null)
        {
            return this.BadRequest("Invalid credentials");
        }

        var token = this.tokenService.GenerateJwtToken(user);
        return this.Ok(token);
    }

    // POST api/account/logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (this.HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var tokenIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (tokenIdClaim != null)
            {
                var tokenId = int.Parse(tokenIdClaim.Value, CultureInfo.InvariantCulture);
                await this.accountService.SignOut(tokenId);
                return this.Ok();
            }
        }

        return this.Unauthorized("Invalid authorization");
    }

    // GET api/account/profile
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        if (this.HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var claims = identity.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return this.Ok(claims);
        }

        return this.Unauthorized("Invalid authorization");
    }
}
