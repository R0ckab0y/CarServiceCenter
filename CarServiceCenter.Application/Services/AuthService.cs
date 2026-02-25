using CarServiceCenter.Application.DTOs.Auth;
using CarServiceCenter.Application.Helpers;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using CarServiceCenter.Application.Configurations; // JwtSettings
using CarServiceCenter.Domain.Enums;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CarServiceCenter.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwt;

        public AuthService(IUserRepository userRepo, IOptions<JwtSettings> jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null || user.PasswordHash != request.Password)
                throw new Exception("Invalid credentials");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1️⃣ Check if user already exists
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            // 2️⃣ Hash password
            var hashedPassword = PasswordHasher.HashPassword(request.Password);

            // 3️⃣ Create User entity
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = request.PhoneNumber,
                Role = Enum.Parse<UserRole>(request.Role, true)
            };

            // 4️⃣ Save to DB
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return new RegisterResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}