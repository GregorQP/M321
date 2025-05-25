using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

app.MapGet("/data", [Microsoft.AspNetCore.Authorization.Authorize]() =>
{
    return Results.Ok(new { data = "Super secret data!" });
});

app.Run();