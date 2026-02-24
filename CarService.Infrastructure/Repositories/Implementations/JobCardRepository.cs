using CarService.Infrastructure.Caching;
using CarService.Infrastructure.Data;
using CarServiceCenter.Application.Interfaces;
using CarServiceCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Repositories.Implementations;

public class JobCardRepository : GenericRepository<JobCard>, IJobCardRepository
{
    public JobCardRepository(AppDbContext context, ICacheService cacheService) : base(context, cacheService)
    {
    }

    public async Task<IEnumerable<JobCard>> GetPendingJobs()
    {
        return await _context.jobCards.Where(x => x.Status == CarServiceCenter.Domain.Enums.JobStatus.Pending)
                    .AsNoTracking().ToListAsync();
    }
}
