using CarServiceCenter.Domain.Entities;
namespace CarServiceCenter.Application.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);

    Task<IEnumerable<Vehicle>> GetByCustomerIdAsync(int customerId);

    Task AddAsync(Vehicle vehicle);

    void Update(Vehicle vehicle);

    void Delete(Vehicle vehicle);

    Task SaveChangesAsync();
}
