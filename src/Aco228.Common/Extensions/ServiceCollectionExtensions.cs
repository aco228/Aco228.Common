using System.Reflection;
using Aco228.Common.Attributes;
using Aco228.Common.Infrastructure;
using Aco228.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Aco228.Common.Extensions;

public static class DynamicDependencyInjectionExtension
{
    private static ConcurrentList<Action<IServiceProvider>> _actions = new();
    private static ConcurrentList<Func<IServiceProvider, Task>> _asyncActions = new();
    
    public static async Task<ServiceProvider> BuildCollection(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        await provider.InitializeAndPrepare();
        return provider;
    }

    public static async Task InitializeAndPrepare(this IServiceProvider provider)
    {
        ServiceProviderHelper.Initialize(provider);
        CoreStateMachine stateMachine = new CoreStateMachine().SetLimit(25);
        stateMachine.OnError = (exception, o) =>
        {
            Console.WriteLine($"!!!! PostExecution.Exception:: {exception}");
        };
        
        foreach (var action in _actions)
            stateMachine.Schedule(async () => action(provider));

        foreach (var action in _asyncActions)
            stateMachine.Schedule(async () => await action(provider));

        await stateMachine.Wait();
        _actions.Clear();
        _asyncActions.Clear();
    }

    public static IServiceCollection RegisterPostBuildAction(this IServiceCollection services, Action<IServiceProvider> action)
    {
        _actions.Add(action);
        return services;
    }

    public static IServiceCollection RegisterPostBuildActionAsync(this IServiceCollection services, Func<IServiceProvider, Task> action)
    {
        _asyncActions.Add(action);
        return services;
    }
    
    
    public static void RegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        try
        {
            var assemblyTypes = assembly.GetTypes();

            foreach (var assemblyType in assemblyTypes)
            {
                if (assemblyType.IsInterface || assemblyType.IsAbstract)
                    continue;

                if (!typeof(IBaseService).IsAssignableFrom(assemblyType))
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

                Console.WriteLine($"[{lifetime}] Registering {inheritInterface.Name}.{assemblyTypeSignature}" +
                                  (hasInjectedServices ? " with injected services" : ""));

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
        catch (Exception ex)
        {
            int a = 0;
        }
    }
}