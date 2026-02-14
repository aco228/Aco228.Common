using System.Collections.Concurrent;

namespace Aco228.Common.Infrastructure;

public static class ServiceExtensionsExtensions
{
    private static ConcurrentDictionary<Type, bool> _isRegisteredMap = new();
    private static bool _isRegistered = false;

    public static void RegisterIfNot(this Type callerType, Action registerAction)
    {
        if(_isRegisteredMap.ContainsKey(callerType)) return;
        registerAction();
        _isRegisteredMap.TryAdd(callerType, true);
    }
}