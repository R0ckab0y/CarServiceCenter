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

    [HttpPost("login")]
    public async Task<LoginResponseDto> Login([FromBody] LoginRequestDto requestDto)
    {
        // No try/catch needed
        // Exceptions are handled globally by ExceptionMiddleware
        return await _service.LoginAsync(requestDto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
    {
        var result = await _service.RegisterAsync(requestDto);
        return Ok(result);
    }
}
