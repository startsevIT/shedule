using Microsoft.EntityFrameworkCore;

namespace Domain.Entity;

public abstract class CrdRepository<T>(IDbContext dbContext) : IRepository<T> where T : Entity
{
    protected DbSet<T> Storage => dbContext.GetDbSet<T>();

    public void Create(T entity)
    {
        Storage.Add(entity);
        dbContext.SaveChanges();
    }

    public T? Read(Guid id)
        =>  Storage.FirstOrDefault(x => x.Id == id);

    public void Delete(Guid id)
    {
        Storage.Remove(Read(id) ?? throw new KeyNotFoundException($"{typeof(T).Name} with id {id} not found"));
        dbContext.SaveChanges();
    }

    protected void SaveChanges()
        => dbContext.SaveChanges();
}