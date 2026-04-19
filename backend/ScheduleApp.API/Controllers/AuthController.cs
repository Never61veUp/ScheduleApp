using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniApp.Server.Security;
using ScheduleApp.API.Security;

namespace ScheduleApp.API.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly TelegramWebAppValidator _validator;
    private readonly JwtTokenService _jwt;
    private readonly IConfiguration _config;
    private readonly ILogger _log;

    public AuthController(
        TelegramWebAppValidator validator,
        JwtTokenService jwt,
        IConfiguration config,
        ILoggerFactory loggerFactory)
    {
        _validator = validator;
        _jwt = jwt;
        _config = config;
        _log = loggerFactory.CreateLogger("MiniApp.Auth.Telegram");
    }

    [HttpPost("auth/telegram")]
    [AllowAnonymous]
    public IActionResult TelegramAuth([FromBody] TelegramAuthRequest body)
    {
        var botToken = _config["TELEGRAM_BOT_TOKEN"];
        if (string.IsNullOrWhiteSpace(botToken))
            return Problem("Missing TELEGRAM_BOT_TOKEN environment variable.", statusCode: 500);

        var result = _validator.ValidateInitData(body.InitData, botToken);
        if (!result.IsValid || result.User is null)
        {
            var initDataLen = body.InitData?.Length ?? 0;
            _log.LogWarning(
                "Telegram auth rejected. reason={Reason} initDataLen={InitDataLen}",
                result.Error ?? "unknown",
                initDataLen);
            return Unauthorized();
        }

        var token = _jwt.CreateToken(result.User, _config);

        return Ok(new TelegramAuthResponse(token, result.User));
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var id = User.FindFirst("tg:id")?.Value;
        var username = User.FindFirst("tg:username")?.Value;
        var firstName = User.FindFirst("tg:first_name")?.Value;
        var lastName = User.FindFirst("tg:last_name")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            id,
            username,
            firstName,
            lastName,
            role
        });
    }
}