using System.Reflection;
using Aco228.Common.Attributes;
using Aco228.Common.Extensions;
using Aco228.Common.Helpers;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;

namespace Aco228.Common;

public static class ServiceProviderHelper
{
    private static IServiceProvider _serviceProvider;

    public static async Task<IServiceProvider> CreateProvider(Type caller, Action<ServiceCollection> impl)
    {
        var builder = new ServiceCollection();
        
        RegisterCoreCommon(caller, builder);
        impl(builder);
        var serviceProvider = await builder.BuildCollection();
        return serviceProvider;
    }

    public static void RegisterCoreCommon(Type caller, IServiceCollection builder)
    {
        Env.Load();
        AssemblyFileLocator.CacheAssemblyFiles(caller.Assembly);
        builder.RegisterServicesFromAssembly(caller.Assembly);
    }

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

    public static T Construct<T>()
    {
        var service = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        foreach (var serviceProp in typeof(T).GetProperties())
        {
            var att = serviceProp.GetCustomAttribute<InjectServiceAttribute>();
            if (att == null)
                continue;
            
            var injectService = GetServiceByType(serviceProp.PropertyType);
            if (injectService == null)
                continue;
            
            serviceProp.SetValue(service, injectService);          
        }
        return service;
    }
    
    public static object ConstructByType(Type type)
    {
        var service = ActivatorUtilities.CreateInstance(_serviceProvider, type);
        foreach (var serviceProp in type.GetProperties())
        {
            var att = serviceProp.GetCustomAttribute<InjectServiceAttribute>();
            if (att == null)
                continue;
            
            var injectService = GetServiceByType(serviceProp.PropertyType);
            if (injectService == null)
                continue;
            
            serviceProp.SetValue(service, injectService);          
        }
        return service;
    }

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