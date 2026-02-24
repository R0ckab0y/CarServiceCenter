using Microsoft.EntityFrameworkCore;
using CarServiceCenter.Domain.Entities;
namespace CarService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> users => Set<User>();
    public DbSet<Vehicle> vehicles => Set<Vehicle>();
    public DbSet<JobCard> jobCards => Set<JobCard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }


}
