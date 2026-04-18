using Microsoft.AspNetCore.HttpOverrides;
using MiniApp.Server.Security;
using Scalar.AspNetCore;
using ScheduleApp.API.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
using var tempProvider = builder.Services.BuildServiceProvider();
var logger = tempProvider.GetRequiredService<ILogger<Program>>();

builder.Configuration.AddEnvironmentVariables();

// Configure Data Protection to persist keys outside container when possible
{
    
    // Location for data protection keys. In production mount a host volume or docker secret here.
    var keysFolder = builder.Configuration["DP_KEYS_FOLDER"] ?? "/keys";
    try
    {
        // Ensure directory exists
        var dir = new System.IO.DirectoryInfo(keysFolder);
        if (!dir.Exists) dir.Create();

        var dpBuilder = builder.Services.AddDataProtection()
            .SetApplicationName("ScheduleApp")
            .PersistKeysToFileSystem(dir);

        // Optionally protect keys with a PFX certificate (path inside container or mounted secret)
        var pfxPath = builder.Configuration["DP_PROTECT_PFX_PATH"];
        if (!string.IsNullOrWhiteSpace(pfxPath))
        {
            try
            {
                var pfxPassword = builder.Configuration["DP_PROTECT_PFX_PASSWORD"];
                var cert = string.IsNullOrEmpty(pfxPassword)
                    ? X509CertificateLoader.LoadCertificateFromFile(pfxPath)
                    : X509CertificateLoader.LoadPkcs12FromFile(pfxPath, pfxPassword, X509KeyStorageFlags.MachineKeySet);

                dpBuilder.ProtectKeysWithCertificate(cert);
            }
            catch (Exception ex)
            {
                // If certificate loading fails, continue but warn (do not throw to avoid killing startup)
                logger.LogWarning($"Warning: failed to load DP protection certificate: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        // If persisting keys fails, leave default behavior but log warning
        logger.LogWarning($"Warning: failed to configure DataProtection key persistence: {ex.Message}");
        if (builder.Environment.IsProduction())
            throw;
    }
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("client-dev", policy =>
        policy
            .WithOrigins("https://localhost:5176", "http://localhost:5176", "https://schedule.mixdev.me")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("client-prod", policy =>
        policy
            .WithOrigins("https://schedule.mixdev.me")
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
// Expose OpenAPI in all environments (used by Scalar UI below).
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "API Documentation";
    options.Theme = ScalarTheme.Default;
    options.AddServer("https://api.schedule.mixdev.me/");
});
app.UseHttpsRedirection(); 

app.UseRouting();
app.UseCors("client-dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok("OK"));
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.Run();