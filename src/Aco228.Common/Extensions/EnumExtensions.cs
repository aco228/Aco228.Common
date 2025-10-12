namespace Aco228.Common.Extensions;

public static class EnumExtensions
{
    public static List<string> GetEnumNames(this Type type)
    {
        if (!type.IsEnum)
            return new List<string>();
        return Enum.GetNames(type).Select(x => x.ToString()).ToList();
    }

    public static bool TryConvertToEnum<T>(this string? input, out T result) 
        where T : struct, Enum
    {
        result = default;
        if (string.IsNullOrEmpty(input))
            return false;

        if (!Enum.TryParse(input, out result))
            return false;

        return true;
    }

    public static object? ToEnum(string input, Type enumType)
    {
        if (!enumType.IsEnum)
            return null;

        if (Enum.TryParse(enumType,  input.ToCharArray(), out var result))
            return result;

        return null;
    }
    
    public static T? ToEnumNull<T>(this string? input) 
        where T : struct, Enum
    {
        if (string.IsNullOrEmpty(input))
            return null;
        
        return Enum.TryParse(input, out T result) ? result : null;
    }
    
    
    public static T ToEnum<T>(this string? input, T defaultValue) 
        where T : struct, Enum
    {
        if (string.IsNullOrEmpty(input))
            return defaultValue;
        
        return Enum.TryParse(input, out T result) ? result : defaultValue;
    }
    
    public static List<T> AsList<T>(this Enum input)
        => Array.AsReadOnly((T[])Enum.GetValues(typeof(T))).ToList();
    
    
    public static List<string> ToStringList<T>(this List<T> enumVal) where T:Enum
    {
        var result = new List<string>();
        foreach (var en in enumVal)
            result.Add(en.ToString());
        return result;
    }

    public static List<T> AddOrUpdate<T>(this List<T> enumList, T enumValue) where T : Enum
    {
        if (enumList.Contains(enumValue))
            enumList.Remove(enumValue);
        else
            enumList.Add(enumValue);
        return enumList;
    }
    
    public static bool TryParseEnum<T>(int value, out T result) where T : struct, Enum
    {
        if (Enum.IsDefined(typeof(T), value))
        {
            result = (T)Enum.ToObject(typeof(T), value);
            return true;
        }

        result = default;
        return false;
    }
}