using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShroomCity.Repositories.DbContext;
using ShroomCity.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShroomCityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("ShroomCity.API")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
