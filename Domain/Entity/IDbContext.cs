using Microsoft.EntityFrameworkCore;

namespace Domain.Entity;

public interface IDbContext
{
    DbSet<T> GetDbSet<T>() where T : Entities.Entity;
    int SaveChanges();
}