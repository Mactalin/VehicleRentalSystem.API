using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Domain.Interface_Repository;
using VehicleRentalSystem.Infrastructure.Data;

namespace VehicleRentalSystem.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // FirstOrDefault respects the soft-delete filter, FindAsync does not
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await _dbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(predicate, ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default) =>
        await _dbSet.AddAsync(entity, ct);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    // AppDbContext turns this into a soft delete on SaveChanges
    public virtual void Remove(T entity) => _dbSet.Remove(entity);
}