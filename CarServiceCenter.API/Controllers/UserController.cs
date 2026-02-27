using System.Security.Claims;
using CarServiceCenter.Application.DTOs.Users;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceCenter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _user;

    public UserController(IUserRepository user)
    {
        _user = user;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _user.GetAllAsync();

        var result = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString()
        });
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _user.GetByIdAsync(userId);
        return Ok(user);
    }
}
