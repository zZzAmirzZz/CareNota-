using System.Linq.Expressions;
using CareNota.Data;
using CareNota.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(ApplicationDbContext Context)
    {
        this.Context = Context;
        DbSet = Context.Set<T>();
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<T?> GetByIdAsync(int Id)
        => await DbSet.FindAsync(Id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await DbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> Predicate)
        => await DbSet.AsNoTracking().Where(Predicate).ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> Predicate)
        => await DbSet.AsNoTracking().FirstOrDefaultAsync(Predicate);

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> Predicate)
        => await DbSet.AnyAsync(Predicate);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? Predicate = null)
        => Predicate is null
            ? await DbSet.CountAsync()
            : await DbSet.CountAsync(Predicate);

    // ── Write ─────────────────────────────────────────────────────────────────

    // EF's DbSet.AddAsync() returns ValueTask<EntityEntry<T>>
    // We await it and discard — the Entity is tracked automatically
    public async Task AddAsync(T Entity)
    {
        await DbSet.AddAsync(Entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> Entities)
    {
        await DbSet.AddRangeAsync(Entities);
    }

    public void Update(T Entity)
        => DbSet.Update(Entity);

    public void Remove(T Entity)
        => DbSet.Remove(Entity);

    public void RemoveRange(IEnumerable<T> Entities)
        => DbSet.RemoveRange(Entities);

    // ── Persistence ───────────────────────────────────────────────────────────

    public async Task<int> SaveChangesAsync()
        => await Context.SaveChangesAsync();

    public List<T> GetAll()
    {
        throw new NotImplementedException();
    }

    public T GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Add(T entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
}