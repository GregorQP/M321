using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey12345678901234567890"))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

string dataFile = "data.json";

// Hilfsfunktion: Daten lesen
List<string> ReadData()
{
    if (!File.Exists(dataFile)) return new();
    return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(dataFile)) ?? new();
}

// Hilfsfunktion: Daten schreiben
void WriteData(List<string> items)
{
    File.WriteAllText(dataFile, JsonSerializer.Serialize(items));
}

// GET: Liste lesen
app.MapGet("/data", [Microsoft.AspNetCore.Authorization.Authorize]() =>
{
    var items = ReadData();
    return Results.Ok(items);
});

// POST: Neuen Eintrag speichern
app.MapPost("/data", [Microsoft.AspNetCore.Authorization.Authorize()] (DataItem item) =>
{
    var items = ReadData();
    items.Add(item.Item);
    WriteData(items);
    return Results.Ok(new { status = "added", item.Item });
});

app.Run("http://0.0.0.0:80");


public record DataItem(string Item);
