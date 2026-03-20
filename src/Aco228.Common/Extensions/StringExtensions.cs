using System.Text;
using MongoDB.Bson;

namespace Aco228.Common.Extensions;

public static class StringExtensions
{
    public static string Remove(this string input, string toRemove)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(toRemove))
            return input;
        
        return input.Replace(toRemove, string.Empty);
    }

    public static string ToSha256(this string randomString)
    {
        var crypt = new System.Security.Cryptography.SHA256Managed();
        var hash = new StringBuilder();
        var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
        foreach (byte theByte in crypto)
            hash.Append(theByte.ToString("x2"));
        return hash.ToString();
    }
    
    public static bool IsValidUrl(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp 
                   || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static string GetStringBetweenCharacters(this string input, char startChar, char endChar, bool onFirstEncounter = false)
    {
        if (string.IsNullOrEmpty(input) || !input.Contains(startChar) && !input.Contains(endChar))
            return string.Empty;
        
        var result = new StringBuilder();
        var inside = false;
        var foundCount = 0;
        
        foreach (var inputChar in input)
        {
            if (inputChar == startChar)
            {
                inside = true;
                foundCount++;
                continue;
            };

            if (inputChar == endChar)
                foundCount--;
            
            if(foundCount == 0 && inside)
                break;
            
            if (inside)
                result.Append(inputChar);
        }
        
        return result.ToString();
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
    
    public static string AddAtEnd(this string input, string? toAdd) => string.IsNullOrEmpty(toAdd) ? input :  input + toAdd;
    public static string AddAtEnd(this string input, ObjectId? toAdd) => toAdd == null ? input :  input + toAdd.ToString();
    
    public static string ToPascalCase(this string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        
        if (string.IsNullOrEmpty(name) || char.IsUpper(name[0])) return name;
        return char.ToUpperInvariant(name[0]) + name.Substring(1);
    }
}