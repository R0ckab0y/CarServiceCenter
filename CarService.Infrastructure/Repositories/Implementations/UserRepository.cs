using CarService.Infrastructure.Caching;
using CarService.Infrastructure.Data;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace CarService.Infrastructure.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ICacheService cacheService) : base(context, cacheService)
    {

    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
    }
}
