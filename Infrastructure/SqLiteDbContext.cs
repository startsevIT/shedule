using System.Collections.Concurrent;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class SqLiteDbContext : DbContext, IDbContext
{
    private readonly ConcurrentDictionary<Type, object> _dbSets = new();

    public SqLiteDbContext()
    {
        Database.EnsureCreated();
    }

    public DbSet<T> GetDbSet<T>() where T : Entity
    {
        var type = typeof(T);
        if (_dbSets.TryGetValue(type, out var dbSet))
            return (DbSet<T>)dbSet;

        _dbSets[type] = Set<T>();
        return (DbSet<T>)_dbSets[type];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=shedule.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var type in AssemblyReflectionExtensions.GetEntities())
            modelBuilder.Entity(type);
    }
}