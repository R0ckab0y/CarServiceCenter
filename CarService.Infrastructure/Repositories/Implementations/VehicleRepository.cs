using CarService.Infrastructure.Caching;
using CarService.Infrastructure.Data;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Repositories.Implementations;

public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext context, ICacheService cacheService) : base(context, cacheService)
    {
    }
    public async Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.vehicles.Where(x => x.CustomerId == customerId)
                    .AsNoTracking().ToListAsync();
    }
}
