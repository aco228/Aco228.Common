namespace Aco228.Common.Extensions;

public static class StringExtensions
{
    public static string Remove(this string input, string toRemove)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(toRemove))
            return input;
        
        return input.Replace(toRemove, string.Empty);
    }
    
    
    public static string GetUntilCharReverse(this string input, char lookFor)
    {
        for(int i = input.Length - 1; i >= 0; i--)
            if (input.GetCharAt(i) == lookFor)
                return input.Substring(i + 1, input.Length - i - 1);
        
        return input;
    }

    public static char? GetCharAt(this string input, int i)
    {
        if (i >= input.Length)
            return null;
        return input.ElementAt(i);
    }
    
    public static string WithZeroPrefix(this int input)
        => (input < 10 ? "0" : "") + input;
    
    public static string ToCamelCase(this string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0])) return name;
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
    
    public static string ToDoubleString(this double input, string extension = "")
        => double.IsNaN(input) || double.IsInfinity(input) 
            ? "0" + extension 
            : (int)input == input ? ((int)input).ToString() + extension : $"{input:0.00}" + extension;
}