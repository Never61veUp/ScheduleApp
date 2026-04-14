using MiniApp.Server.Security;
using Scalar.AspNetCore;
using ScheduleApp.API.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("client-dev", policy =>
        policy
            .WithOrigins("https://localhost:5176", "http://localhost:5176")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services
    .AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["JWT_ISSUER"] ?? "MiniApp";
        var audience = builder.Configuration["JWT_AUDIENCE"] ?? "MiniApp";
        var key = builder.Configuration["JWT_KEY"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing JWT signing key. Set environment variable JWT_KEY.");

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<TelegramWebAppValidator>();
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "API Documentation";
    options.Theme = ScalarTheme.Default;
    // if(!app.Environment.IsDevelopment())
    //     options.AddServer("https://api.reviewanalyzer.mixdev.me");
});

app.UseHttpsRedirection();

app.UseCors("client-dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();