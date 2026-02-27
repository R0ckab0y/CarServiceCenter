using CarServiceCenter.Application.DTOs.Auth;
using CarServiceCenter.Application.Helpers;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Application.Configurations;
using CarServiceCenter.Domain.Entities;
using CarServiceCenter.Domain.Enums;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarServiceCenter.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly JwtSettings _jwt;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshTokenRepo,
            IOptions<JwtSettings> jwt,
            ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _jwt = jwt.Value;
            _logger = logger;
        }

        // ================================
        // REGISTER
        // ================================
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            _logger.LogInformation("Register attempt for email: {Email}", request.Email);

            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed — Email already exists: {Email}", request.Email);
                throw new Exception("Email already registered");
            }

            var hashedPassword = PasswordHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                PhoneNumber = request.PhoneNumber,
                Role = Enum.Parse<UserRole>(request.Role, true)
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);

            return new RegisterResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        // ================================
        // LOGIN
        // ================================
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed — user not found");
                return null;
            }

            var isValid = VerifyPassword(request.Password, user.PasswordHash);
            if (!isValid)
            {
                _logger.LogWarning("Login failed — invalid password");
                return null;
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepo.AddAsync(refreshEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            _logger.LogInformation("Login successful for UserId: {UserId}", user.Id);

            return new LoginResponseDto
            {
                Token = accessToken,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Expiration = DateTime.UtcNow.AddHours(2)
            };
        }

        // ================================
        // REFRESH TOKEN
        // ================================
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Refresh token requested");

            var tokenEntity = await _refreshTokenRepo.GetByTokenAsync(refreshToken);

            if (tokenEntity == null || tokenEntity.ExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                throw new Exception("Invalid or expired refresh token");
            }

            var user = await _userRepo.GetByIdAsync(tokenEntity.UserId);
            if (user == null)
                throw new Exception("User not found");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            tokenEntity.Token = newRefreshToken;
            tokenEntity.ExpiryDate = DateTime.UtcNow.AddDays(7);

            await _refreshTokenRepo.AddAsync(tokenEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            _logger.LogInformation("Token refreshed for UserId: {UserId}", user.Id);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        // ================================
        // JWT TOKEN GENERATOR
        // ================================
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

        // ================================
        // PASSWORD VERIFY
        // ================================
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

        // ================================
        // REFRESH TOKEN GENERATOR
        // ================================
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}