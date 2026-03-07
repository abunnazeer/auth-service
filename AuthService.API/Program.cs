using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Infrastructure.Auth;
using AuthService.API.Contracts.Auth;
using AuthService.Infrastructure.Configuration;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapPost("/auth/register", async (
    RegisterRequest request,
    IAuthService authService,
    CancellationToken ct) =>
{
    try
    {
        await authService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Role,
            ct);

        return Results.Ok();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/auth/login", async (
    LoginRequest request,
    IAuthService authService,
    CancellationToken ct) =>
{
    var token = await authService.LoginAsync(
        request.Email,
        request.Password,
        ct);

    if (token is null)
        return Results.Unauthorized();

    return Results.Ok(new { accessToken = token });
});

app.MapGet("/me", (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
        email = user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value,
        role = user.FindFirst(ClaimTypes.Role)?.Value
    });
}).RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();
app.Run();
