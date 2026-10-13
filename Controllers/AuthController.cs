using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Models;
using ServiceAggregator.Api.Services;
namespace ServiceAggregator.Api.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginService _auth;
    private readonly JwtService _jwt;
    private readonly SkiaSharpCaptchaService _skiaSharpCaptchaService;
    private readonly JwtSettings _jwtSettings;
    public AuthController(LoginService auth, JwtService jwt
        , SkiaSharpCaptchaService skiaSharpCaptchaService, IOptions<JwtSettings> jwtOption
        )
    {
        _auth = auth;
        _jwt = jwt;
        _skiaSharpCaptchaService = skiaSharpCaptchaService;
        _jwtSettings = _jwtSettings;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login(LoginRequest request)
    {
        var role = _auth.Validate(request.Username, request.Password);
        if (role == null)
            return Unauthorized();

        string token = _jwt.GenerateToken(request.Username, role);

        return Ok(new LoginResponse
        {
            Token = token,
            Username = request.Username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes)
        });
    }

    [HttpPost("loginwithcaptcha")]
    [AllowAnonymous]
    public IActionResult LoginWithCaptcha(LoginWithCaptchaRequest request)
    {
        var isValid = _skiaSharpCaptchaService.ValidateCaptcha(request.Key, request.Captcha);
        if (!isValid)
            return Ok(new { IsSuccess = false, StatusCode = 2, Message = "کد امنیتی را صحیح وارد کنید" });

        var role = _auth.Validate(request.Username, request.Password);
        if (role == null)
            return Unauthorized();

        string token = _jwt.GenerateToken(request.Username, role);

        return Ok(new LoginResponse
        {
            Token = token,
            Username = request.Username,
            Role = role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }
    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        return Ok();
    }
    [HttpGet("captcha")]
    [AllowAnonymous]
    public IActionResult GenerateCaptcha()
    {
        try
        {
            Console.WriteLine("Start GenerateCaptcha");
            var (image, key, text) = _skiaSharpCaptchaService.GenerateCaptchaImage();
            return Ok(new { image, key });
        }
        catch (Exception ex)
        {
            return Ok(ex.Message);
        }
    }
}
