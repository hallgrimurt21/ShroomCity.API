using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<ShroomCityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("ShroomCity.API")));

services.AddControllers();

services.AddAuthorization(options =>
{
    options.AddPolicy("read:mushrooms", policy => policy.RequireClaim("Permission", "read:mushrooms"));
    options.AddPolicy("write:mushrooms", policy => policy.RequireClaim("Permission", "write:mushrooms"));
    options.AddPolicy("read:researchers", policy => policy.RequireClaim("Permission", "read:researchers"));
    options.AddPolicy("write:researchers", policy => policy.RequireClaim("Permission", "write:researchers"));
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = builder.Configuration["Jwt:SigningKey"] != null ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)) : null!
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                if (context.SecurityToken is JwtSecurityToken token && int.TryParse(token.Id, out var tokenId) && await tokenService.IsTokenBlacklisted(tokenId))
                {
                    context.Fail("This token is blacklisted.");
                }
            }
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c
    => c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShroomCity API", Version = "v1" }));

services.AddScoped<IAccountService, AccountService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IMushroomService, MushroomService>();
services.AddScoped<IResearcherService, ResearcherService>();
services.AddScoped<IExternalMushroomService, ExternalMushroomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShroomCity API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
