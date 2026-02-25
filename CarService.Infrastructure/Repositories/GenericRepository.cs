using System.Linq.Expressions;
using CarService.Infrastructure.Caching;
using CarService.Infrastructure.Data;
using CarServiceCenter.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _DbSet;
    private readonly ICacheService _cacheService;


    private string GetCacheKey(string key)
    {
        return $"{typeof(T).Name}_{key}";
    }

    public GenericRepository(AppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _DbSet = context.Set<T>();
        _cacheService = cacheService;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var cacheKey = GetCacheKey($"Id_{id}");

        var cached = await _cacheService.GetAsync<T>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _DbSet.FindAsync(id);
        if (entity != null)
        {
            await _cacheService.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(5));
        }

        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var cacheKey = GetCacheKey("All");

        var cached = await _cacheService.GetAsync<IEnumerable<T>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var data = await _DbSet.AsNoTracking().ToListAsync();
        await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5));
        return data;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _DbSet.AsNoTracking();

        var totalCount = await query.CountAsync();

        var data = await query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

        return (data, totalCount);
    }

    public async Task AddAsync(T entity)
    {
        await _DbSet.AddAsync(entity);
        _cacheService.Remove(GetCacheKey("All"));
    }

    public void Update(T entity)
    {
        _DbSet.Update(entity);
        _cacheService.Remove(GetCacheKey("All"));
    }

    public void Delete(T entity)
    {
        _DbSet.Remove(entity);
        _cacheService.Remove(GetCacheKey("All"));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
