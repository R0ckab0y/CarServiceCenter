using System.Linq.Expressions;
namespace CarServiceCenter.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    // Get by ID
    Task<T?> GetByIdAsync(int id);

    // Get all
    Task<IEnumerable<T>> GetAllAsync();

    // Find with filter
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Pagination
    Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize);

    // Add
    Task AddAsync(T entity);

    // Update
    void Update(T entity);

    // Delete
    void Delete(T entity);

}
