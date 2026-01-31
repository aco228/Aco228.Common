using System.Reflection;
using Aco228.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Aco228.Common.Extensions;

public static class DynamicDependencyInjectionExtension
{
    public static void RegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var assemblyTypes = assembly.GetTypes();
        
        foreach (var assemblyType in assemblyTypes)
        {
            if (assemblyType.IsInterface)
                continue;
            
            if(!typeof(IBaseService).IsAssignableFrom(assemblyType))
                continue;

            var interfaceTypes = assemblyType.GetInterfaces();
            var inheritInterface = interfaceTypes.FirstOrDefault(x => x.IsInterface);
            
            if(typeof(IScoped).IsAssignableFrom(inheritInterface))
                services.AddScoped(inheritInterface, assemblyType);
            
            if(typeof(ITransient).IsAssignableFrom(inheritInterface))
                services.AddTransient(inheritInterface, assemblyType);
            
            if(typeof(ISingleton).IsAssignableFrom(inheritInterface))
                services.AddSingleton(inheritInterface, assemblyType);
            
        }
    }
}