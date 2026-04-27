using Microsoft.EntityFrameworkCore;

namespace Domain.Entity;

public interface IDbContext
{
    DbSet<T> GetDbSet<T>() where T : Entity;
    int SaveChanges();
}