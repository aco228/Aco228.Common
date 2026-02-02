using System.Reflection;
using Aco228.Common.Attributes;
using Aco228.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Aco228.Common.Extensions;

public static class DynamicDependencyInjectionExtension
{
    private static List<Action<ServiceProvider>> _actions = new();
    
    public static ServiceProvider BuildCollection(this IServiceCollection services)
    {
        var provider =  services.BuildServiceProvider();
        ServiceProviderHelper.Initialize(provider);

        foreach (var action in _actions)
            action(provider);
        
        return provider;
    }

    public static IServiceCollection RegisterPostBuildAction(this IServiceCollection services, Action<ServiceProvider> action)
    {
        _actions.Add(action);
        return services;
    }
    
    
    public static void RegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var assemblyTypes = assembly.GetTypes();
        
        foreach (var assemblyType in assemblyTypes)
        {
            if (assemblyType.IsInterface || assemblyType.IsAbstract)
                continue;
            
            if(!typeof(IBaseService).IsAssignableFrom(assemblyType))
                continue;

            var interfaceTypes = assemblyType.GetInterfaces();
            var inheritInterface = interfaceTypes
                .Where(x => typeof(IScoped).IsAssignableFrom(x) || 
                            typeof(ITransient).IsAssignableFrom(x) || 
                            typeof(ISingleton).IsAssignableFrom(x))
                .OrderByDescending(x => x.GetInterfaces().Length)
                .FirstOrDefault();

            if (inheritInterface == null)
                continue;

            Dictionary<PropertyInfo, Type> injectableServices = new();
            bool hasInjectedServices = false;
            foreach (var propertyInfo in assemblyType.GetProperties())
            {
                var injectedServiceAttribute = propertyInfo.GetCustomAttribute<InjectServiceAttribute>();
                if (injectedServiceAttribute != null)
                {
                    hasInjectedServices = true;
                    injectableServices.Add(propertyInfo, propertyInfo.PropertyType);
                }
            }
            
            ServiceLifetime lifetime = ServiceLifetime.Transient;
            string assemblyTypeSignature = $"{assembly.FullName?.Split(",").First()}::{assemblyType.Name}";

            if (interfaceTypes.Contains(typeof(IScoped)))
                lifetime = ServiceLifetime.Scoped;
            else if (interfaceTypes.Contains(typeof(ITransient)))
                lifetime = ServiceLifetime.Transient;
            else if (interfaceTypes.Contains(typeof(ISingleton)))
                lifetime = ServiceLifetime.Singleton;
            else
            {
                Console.WriteLine($"Could not register {assemblyTypeSignature}");
                continue;
            }
            
            Console.WriteLine($"Registering {inheritInterface.Name}.{assemblyTypeSignature}" + (hasInjectedServices ? " with injected services" : ""));

            Func<IServiceProvider, object>? implementationFactory = null;
            if (hasInjectedServices)
                implementationFactory = (pr) =>
                {
                    var service = ActivatorUtilities.CreateInstance(pr, assemblyType);
                    
                    foreach (var (propertyInfo, type) in injectableServices)
                        propertyInfo.SetValue(service, pr.GetService(type));
                    
                    return service;
                };
            
            var descriptor = hasInjectedServices && implementationFactory != null
                ? new ServiceDescriptor(inheritInterface, implementationFactory, lifetime)
                : new ServiceDescriptor(inheritInterface, assemblyType, lifetime);
            
            services.Add(descriptor);
            
        }
    }
}