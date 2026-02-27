using CarServiceCenter.Application.DTOs.Auth;
using CarServiceCenter.Application.Helpers;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using CarServiceCenter.Application.Configurations; // JwtSettings
using CarServiceCenter.Domain.Enums;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user == null)
                return null;

            var isValid = VerifyPassword(request.Password, user.PasswordHash);

            if (!isValid)
                return null;

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Expiration = DateTime.UtcNow.AddHours(2)
            };
        }
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var enteredHash = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            return hash.SequenceEqual(enteredHash);
        }
    }
}