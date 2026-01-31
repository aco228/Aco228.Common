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
            var inheritInterface = interfaceTypes
                .Where(x => typeof(IScoped).IsAssignableFrom(x) || 
                            typeof(ITransient).IsAssignableFrom(x) || 
                            typeof(ISingleton).IsAssignableFrom(x))
                .OrderByDescending(x => x.GetInterfaces().Length)
                .FirstOrDefault();

            string assemblyTypeSignature = $"{assembly.FullName?.Split(",").First()}::{assemblyType.Name}";
            string type = "";

            if (interfaceTypes.Contains(typeof(IScoped)))
            {
                type = "SCOPED";
                services.AddScoped(inheritInterface, assemblyType);
            }
            if (interfaceTypes.Contains(typeof(ITransient)))
            {
                type = "TRANSIENT";
                services.AddTransient(inheritInterface, assemblyType);
            }
            if (interfaceTypes.Contains(typeof(ISingleton)))
            {
                type = "SINGLETON";
                services.AddSingleton(inheritInterface, assemblyType);
            }
            else
            {
                Console.WriteLine($"Could not register {assemblyTypeSignature}");
                continue;
            }
            
            Console.WriteLine($"Registering [{type}] - {assemblyTypeSignature}");
            
        }
    }
}