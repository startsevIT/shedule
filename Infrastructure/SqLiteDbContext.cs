using System.Collections.Concurrent;
using Domain.Entity;
using Domain.Entity.Entities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public sealed class SqLiteDbContext : DbContext, IDbContext
{
    private readonly ConcurrentDictionary<Type, object> _dbSets = new();

    public SqLiteDbContext(IOptions<AdminCredentials> credentialsOptions)
    {
        if (Database.EnsureCreated())
        {
            var users = GetDbSet<User>();
            users.Add(User.CreateAdmin(credentialsOptions.Value));
            SaveChanges();
        }
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

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(SnakeCaseNamingConverter.ToSnakeCase(entityType.GetTableName()));

            foreach (var property in entityType.GetProperties())
                property.SetColumnName(SnakeCaseNamingConverter.ToSnakeCase(property.GetColumnName()));
        }
    }
}