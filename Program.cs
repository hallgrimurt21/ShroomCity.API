using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShroomCity.API.Extensions;
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

services.AddSwaggerConfiguration();

services.AddRepositories();
services.AddServices();
services.AddHttpClient();

var app = builder.Build();
var env = app.Environment;

app.ConfigureApiMiddleware(env);

app.Run();
