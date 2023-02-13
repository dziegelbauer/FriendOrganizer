using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.UI.Data.Repositories;

public class GenericRepository<TEntity, TDbContext> : IGenericRepository<TEntity>
  where TDbContext : DbContext
  where TEntity : class
{
  protected readonly TDbContext _dbContext;

  protected GenericRepository(TDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public virtual async Task<TEntity> GetByIdAsync(int id)
  {
    return await _dbContext.Set<TEntity>().FindAsync(id);
  }

  public async Task<IEnumerable<TEntity>> GetAllAsync()
  {
    return await _dbContext.Set<TEntity>().ToListAsync();
  }

  public async Task SaveAsync()
  {
    await _dbContext.SaveChangesAsync();
  }

  public bool HasChanges()
  {
    return _dbContext.ChangeTracker.HasChanges();
  }

  public void Add(TEntity model)
  {
    _dbContext.Set<TEntity>().Add(model);
  }

  public void Remove(TEntity model)
  {
    _dbContext.Set<TEntity>().Remove(model);
  }
}