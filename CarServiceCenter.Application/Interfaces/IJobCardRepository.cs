using CarServiceCenter.Domain.Entities;
namespace CarServiceCenter.Application.Interfaces;

public interface IJobCardRepository
{
    Task<JobCard?> GetByIdAsync(int id);

    Task<IEnumerable<JobCard>> GetPendingJobs();


    Task AddAsync(JobCard jobCard);

    void Update(JobCard jobCard);

    Task SaveChangesAsync();
}
