using CarServiceCenter.Application.DTOs.Auth;
using CarServiceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // ========================
    // LOGIN
    // ========================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        var result = await _service.LoginAsync(requestDto);

        if (result == null)
            return Unauthorized("Invalid email or password");

        return Ok(result);
    }

    // ========================
    // REGISTER
    // ========================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
    {
        var result = await _service.RegisterAsync(requestDto);

        return Ok(new
        {
            message = "User registered successfully!",
            data = result
        });
    }

    // ========================
    // REFRESH TOKEN
    // ========================
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
    {
        var result = await _service.RefreshTokenAsync(requestDto.Token);
        return Ok(result);
    }
}