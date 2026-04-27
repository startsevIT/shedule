using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity;

public abstract class Entity
{
    [Column("creation_time")] public DateTime CreationTime { get; set; } = DateTime.Now;
    [Column("id")] public Guid Id { get; private set; } = Guid.NewGuid();
}

public static class AssemblyReflectionExtensions
{
    public static List<Type> GetEntities()
        => GetAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(Entity)))
            .ToList();

    public static List<Type> GetServices()
        => GetAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false }
                        && t.Name.EndsWith("Service")
                        && t.Namespace != null && t.Namespace.Contains("Services"))
            .ToList();

    public static List<(Type Implementation, Type Interface)> GetRepositories()
    {
        var allTypes = GetAssemblies().ToList();

        var repositoryInterfaces = allTypes
            .Where(t => t.IsInterface && t.Name.EndsWith("Repository"))
            .ToList();

        var result = new List<(Type, Type)>();

        foreach (var repositoryInterface in repositoryInterfaces)
        {
            var implementations = allTypes
                .Where(t => t is { IsClass: true, IsAbstract: false } && repositoryInterface.IsAssignableFrom(t))
                .ToList();

            if(implementations.Count == 0)
                throw new Exception("No implementations found for " + repositoryInterface.Name);

            result.Add((implementations[0], repositoryInterface));
        }

        return result;
    }

    private static IEnumerable<Type> GetAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes());
}