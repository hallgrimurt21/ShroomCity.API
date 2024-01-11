using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShroomCity.API.Extensions;
using ShroomCity.Models.Constants;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Repositories.Implementations;
using ShroomCity.Repositories.Interfaces;
using ShroomCity.Services.Implementations;
using ShroomCity.Services.Interfaces;
using ShroomCity.Utilities.Exceptions;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var jwtConfig = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>() ?? throw new JwtConfigMissingException();

services.AddSingleton(jwtConfig);

services.AddDbContext<ShroomCityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("ShroomCity.API")));

services.AddControllers();

services.AddAuthorization(options =>
{
    options.AddPolicy("read:mushrooms", policy => policy.RequireClaim("permissions", "read:mushrooms"));
    options.AddPolicy("write:mushrooms", policy => policy.RequireClaim("permissions", "write:mushrooms"));
    options.AddPolicy("read:researchers", policy => policy.RequireClaim("permissions", "read:researchers"));
    options.AddPolicy("write:researchers", policy => policy.RequireClaim("permissions", "write:researchers"));
});

services.AddJwtAuthentication(configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

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

services.AddScoped<IAccountRepository, AccountRepository>();
services.AddScoped<ITokenRepository, TokenRepository>();
services.AddScoped<IMushroomRepository, MushroomRepository>();
services.AddScoped<IResearcherRepository, ResearcherRepository>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IMushroomService, MushroomService>();
services.AddScoped<IResearcherService, ResearcherService>();
services.AddHttpClient<IExternalMushroomService, ExternalMushroomService>();

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
