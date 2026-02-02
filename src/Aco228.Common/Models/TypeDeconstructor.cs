namespace Aco228.Common.Models;

public class TypeDeconstructor
{
    public bool IsNullable { get; internal set; }
    public bool IsList { get; internal set; }
    public Type Type { get; internal set; }
    
    public string Name => Type.Name;
    
    public bool IsClass =>
        Type.IsClass &&
        !Type.IsAbstract &&
        Type != typeof(string) &&
        !Type.IsArray &&
        !typeof(Delegate).IsAssignableFrom(Type);

    public TypeDeconstructor(bool isList, bool isNullable,  Type type)
    {
        IsNullable = isNullable;
        IsList = isList;
        Type = type;
    }
    
    public static TypeDeconstructor Get(Type type, bool isNullable = false)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return new(true, isNullable, type.GetGenericArguments()[0]);
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return Get(type.GetGenericArguments()[0], true);
        
        return new (false, isNullable, type);
    }
}