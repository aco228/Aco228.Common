using Microsoft.Extensions.DependencyInjection;

namespace Aco228.Common;

public static class ServiceProviderHelper
{
    private static IServiceProvider _serviceProvider;
    
    public static void Initialize(IServiceProvider provider)
    {
        _serviceProvider = provider;
    }

    public static object? GetServiceByType(Type type)
    {
        try
        {
            return _serviceProvider.GetService(type);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static dynamic? GetDynamicServiceByType(Type type)
    {
        try
        {
            return _serviceProvider.GetService(type);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static T? GetService<T>()
        => _serviceProvider.GetService<T>() ?? default;

    public static T? GetScopedService<T>()
    {
        using var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetService<T>();
    }
    
    public static T Cast<T>(object o)
    {
        return (T)o;
    }
}