namespace Domain.Entity;

public static class AssemblyReflectionExtensions
{
    public static List<Type> GetEntities()
        => GetAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(Entities.Entity)))
            .ToList();

    public static List<Type> GetServices()
        => GetAssemblies()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false }
                        && t.Name.EndsWith("Service")
                        && t.Namespace != null && t.Namespace.Contains("Services"))
            .ToList();

    private static IEnumerable<Type> GetAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes());
}