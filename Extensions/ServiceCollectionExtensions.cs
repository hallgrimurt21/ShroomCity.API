namespace ShroomCity.API.Extensions;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShroomCity.Models.Constants;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IMushroomRepository, MushroomRepository>();
        services.AddScoped<IResearcherRepository, ResearcherRepository>();

        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IAccountService, AccountService>();
        _ = services.AddScoped<ITokenService, TokenService>();
        _ = services.AddScoped<IMushroomService, MushroomService>();
        _ = services.AddScoped<IResearcherService, ResearcherService>();
        _ = services.AddScoped<IExternalMushroomService, ExternalMushroomService>();

        return services;
    }
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShroomCity API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            options.AddPolicy("read:mushrooms", policy => policy.RequireClaim("permissions", "read:mushrooms"));
            options.AddPolicy("write:mushrooms", policy => policy.RequireClaim("permissions", "write:mushrooms"));
            options.AddPolicy("read:researchers", policy => policy.RequireClaim("permissions", "read:researchers"));
            options.AddPolicy("write:researchers", policy => policy.RequireClaim("permissions", "write:researchers"));
        });

        return services;
    }
    public static IServiceCollection AddShroomCityDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<ShroomCityDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("ShroomCity.API")));

        return services;
    }
}
