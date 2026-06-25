using Microsoft.EntityFrameworkCore;

namespace Domain.Entity;

public class EntityStorage(IDbContext dbContext)
{
    //Create
    public void Create<T>(T entity) where T : Entities.Entity
        => Create([entity]);

    public void Create<T>(IEnumerable<T> entities) where T : Entities.Entity
        => GetDbSet<T>()
            .AddRange(entities);

    //Read
    public IQueryable<T> Select<T>() where T : Entities.Entity
        => GetDbSet<T>().AsQueryable();

    public T[] Read<T>(IEnumerable<Guid> ids) where T : Entities.Entity
        => GetDbSet<T>()
            .Where(x => ids.Contains(x.Id))
            .ToArray();

    public T? Read<T>(Guid id) where T : Entities.Entity
        => Read<T>([id])
            .SingleOrDefault();

    //Update
    public void Update<T>(T chat) where T : Entities.Entity
        => GetDbSet<T>()
            .Update(chat);

    private DbSet<T> GetDbSet<T>() where T : Entities.Entity
        => dbContext.GetDbSet<T>();

    //Delete
    public void Delete<T>(IEnumerable<T> entities) where T : Entities.Entity
        => GetDbSet<T>()
            .RemoveRange(entities);
}