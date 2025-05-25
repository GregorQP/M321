using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

var builder = WebApplication.CreateBuilder();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

string key = "supersecretkey12345678901234567890";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

app.MapPost("/login", (HttpRequest request) =>
{
    var username = request.Query["username"];
    var password = request.Query["password"];

    if (username != "admin" || password != "1234")
        return Results.Unauthorized();

    var token = new JwtSecurityToken(
        claims: new[] { new Claim(ClaimTypes.Name, username) },
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});

app.Run("http://0.0.0.0:80");
