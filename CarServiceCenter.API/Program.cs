using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Application.Services;
using CarServiceCenter.Application.Configurations;
using CarService.Infrastructure.Data;
using CarService.Infrastructure.Repositories.Implementations;
using CarService.Infrastructure.Caching;
using CarServiceCenter.API.Middlewares;
using AspNetCoreRateLimit;
using Serilog;


Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .WriteTo.File("logs/log-.txt",
             rollingInterval: RollingInterval.Day)
             .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1️⃣ Configure Services
// =======================

//Logger
builder.Host.UseSerilog();


// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IJobCardRepository, JobCardRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
// Caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();

//RateLimiter
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting")
);
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Application Services
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IAuthService, AuthService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

// =======================
// 2️⃣ Build App
// =======================
var app = builder.Build();

// =======================
// 3️⃣ Configure Middleware
// =======================
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

// =======================
// 4️⃣ Map Root Health Check
// =======================
app.MapGet("/", () => Results.Ok(new
{
    status = "Healthy",
    message = "CarServiceCenter API is running!",
    timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck");

// =======================
// 5️⃣ Map Controllers
// =======================
app.MapControllers();

// =======================
// 6️⃣ Run App
// =======================
app.Run();