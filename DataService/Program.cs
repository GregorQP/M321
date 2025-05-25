using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey12345"))
        };
    });

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
app.MapPost("/data", [Microsoft.AspNetCore.Authorization.Authorize()] (string item) =>
{
    var items = ReadData();
    items.Add(item);
    WriteData(items);
    return Results.Ok(new { status = "added", item });
});

app.Run();
