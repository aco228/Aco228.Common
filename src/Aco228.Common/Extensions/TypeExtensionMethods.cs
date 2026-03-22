using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Aco228.Common.Extensions;

public static class TypeExtensionMethods
{
    public static List<(PropertyInfo Info, T? Attribute)> GetPropertyWithAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetProperties()
            .Select(x => (Info: x, Attribute: GetAttribute<T>(x)))
            .Where(x => x.Attribute != default && x.Info.CanWrite)
            .ToList(); 
    }

    public static T? GetAttribute<T>(this Enum enumVal) where T:System.Attribute
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }
    
    public static T? GetAttribute<T>(this PropertyInfo info) where T : Attribute
    {
        return (T?)info.GetCustomAttributes(typeof(T), true)
            .FirstOrDefault();
    }
    
    public static bool IsSimple(this Type type) 
        => TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
    
    public static bool IsClassObject(this Type type)
    {
        if (!type.IsClass || type.IsAbstract)
            return false;

        if (type == typeof(string))
            return false;

        if (type.IsArray)
            return false;

        if (typeof(Delegate).IsAssignableFrom(type))
            return false;

        // 👇 THIS is what you’re missing
        if (typeof(IEnumerable).IsAssignableFrom(type))
            return false;

        return true;
    }
       
    public static Type[] GetTypesInNamespace(this Assembly assembly, string nameSpace)
    {
        return 
            assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                .ToArray();
    }
    
    public static object CastObject(this object input, Type to)
    {
        try
        {
            return TypeDescriptor.GetConverter(to).ConvertFrom(input.ToString());
        }
        catch(Exception ex)
        {
            try
            {
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(input), to);
            }
            catch (Exception jsonEx)
            {
                return null;   
            }
        }
    }
    
    
    public static T DeepClone<T>(this T obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json)!;
    }
    
    private static readonly NullabilityInfoContext _context = new();
    public static bool IsNullableReference(this PropertyInfo property)
    {
        var nullability = _context.Create(property);
        return nullability.ReadState == NullabilityState.Nullable;
    }
}