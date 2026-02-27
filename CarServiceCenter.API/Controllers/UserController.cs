using CarServiceCenter.Application.DTOs.Users;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceCenter.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _user;

    public UserController(IUserRepository user)
    {
        _user = user;
    }

    [HttpGet]
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
}
