namespace ShroomCity.API.Extensions;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Models.Constants;
using ShroomCity.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtConfiguration:Issuer"],
                    ValidAudience = configuration["JwtConfiguration:Audience"],
                    IssuerSigningKey = configuration["JwtConfiguration:Secret"] != null ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfiguration:Secret"])) : null
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                        if (context.SecurityToken is JwtSecurityToken token)
                        {
                            var tokenIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypeConstants.TokenIdClaimType);
                            if (tokenIdClaim != null && int.TryParse(tokenIdClaim.Value, out var tokenId) && await tokenService.IsTokenBlacklisted(tokenId))
                            {
                                context.Fail("This token is blacklisted.");
                            }
                        }
                    }
                };
            });

        return services;
    }
}
