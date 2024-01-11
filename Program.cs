using ShroomCity.API.Extensions;
using ShroomCity.Services.Implementations;
using ShroomCity.Utilities.Exceptions;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var jwtConfig = configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>() ?? throw new JwtConfigMissingException();

services.AddSingleton(jwtConfig);

services.AddControllers();

// ServiceCollectionExtensions
services.AddShroomCityDbContext(configuration)
        .AddAuthorizationPolicies()
        .AddJwtAuthentication(configuration)
        .AddSwaggerConfiguration()
        .AddRepositories()
        .AddServices()
        .AddHttpClient();

var app = builder.Build();
var env = app.Environment;

// ApplicationBuilderExtensions
app.ConfigureApiMiddleware(env);

app.Run();
