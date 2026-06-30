namespace Aco228.Common.Helpers;

using System.Reflection;

public static class AssemblyClassLocator
{
    private static readonly object Lock = new();
    private static List<Assembly>? _cachedAssemblies;

    /// <summary>
    /// Finds all concrete classes assignable to T (works for interfaces and non-generic base classes).
    /// For open generic base types (e.g. RequestTask&lt;&gt;), use FindAllDerivedFrom instead.
    /// </summary>
    public static List<Type> FindAll<T>(string? assemblyPrefixFilter = null, bool includeAbstract = false)
    {
        var target = typeof(T);
        return GetAssemblies(assemblyPrefixFilter)
            .SelectMany(GetLoadableTypes)
            .Where(t => t != target
                        && !t.IsInterface
                        && (includeAbstract || !t.IsAbstract)
                        && IsAssignableToType(t, target))
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Finds all concrete classes derived from a base type, including open generic base types
    /// like typeof(RequestTask&lt;&gt;) or typeof(DistributedRequestTask&lt;&gt;).
    /// </summary>
    public static List<Type> FindAllDerivedFrom(Type baseType, string? assemblyPrefixFilter = null, bool includeAbstract = false)
    {
        return GetAssemblies(assemblyPrefixFilter)
            .SelectMany(GetLoadableTypes)
            .Where(t => t != baseType
                        && !t.IsInterface
                        && (includeAbstract || !t.IsAbstract)
                        && IsAssignableToType(t, baseType))
            .Distinct()
            .ToList();
    }

    private static bool IsAssignableToType(Type candidate, Type targetType)
    {
        // Direct match: handles non-generic base classes and interfaces (TaskBase, ITask)
        if (targetType.IsAssignableFrom(candidate))
            return true;

        // From here on, only open generic type definitions need special handling
        if (!targetType.IsGenericTypeDefinition)
            return false;

        if (targetType.IsInterface)
        {
            foreach (var i in candidate.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == targetType)
                    return true;
            }
            return false;
        }

        // Walk up the base class chain comparing generic type definitions
        // (handles RequestTask<>, DistributedRequestTask<>, etc.)
        var current = candidate.BaseType;
        while (current != null && current != typeof(object))
        {
            var currentDef = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
            if (currentDef == targetType)
                return true;

            current = current.BaseType;
        }

        return false;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }

    private static List<Assembly> GetAssemblies(string? assemblyPrefixFilter)
    {
        lock (Lock)
        {
            if (_cachedAssemblies == null)
            {
                LoadReferencedAssemblies(
                    Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(),
                    assemblyPrefixFilter);

                _cachedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            }
        }

        return assemblyPrefixFilter == null
            ? _cachedAssemblies
            : _cachedAssemblies
                .Where(a => a.GetName().Name?.StartsWith(assemblyPrefixFilter, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
    }

    private static void LoadReferencedAssemblies(Assembly root, string? prefixFilter)
    {
        var visited = new HashSet<string> { root.GetName().Name! };
        var queue = new Queue<Assembly>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var asm = queue.Dequeue();
            foreach (var refName in asm.GetReferencedAssemblies())
            {
                if (!visited.Add(refName.Name!))
                    continue;

                // Skip framework/3rd-party noise unless you want everything loaded
                if (prefixFilter != null &&
                    !refName.Name!.StartsWith(prefixFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    queue.Enqueue(Assembly.Load(refName));
                }
                catch
                {
                    // unresolvable reference (native, missing, etc.) - skip
                }
            }
        }
    }

    /// <summary>Call if assemblies get loaded dynamically after first scan and you need a fresh result.</summary>
    public static void ClearCache()
    {
        lock (Lock)
        {
            _cachedAssemblies = null;
        }
    }
}