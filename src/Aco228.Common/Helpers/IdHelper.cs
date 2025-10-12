using System.Text;
using Aco228.Common.Extensions;

namespace Aco228.Common.Helpers;

public static class IdHelper
{
    private static readonly char[] _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    private static readonly Random _random = new Random();
    
    public static string GetId(string prefix = "")
        => (string.IsNullOrEmpty(prefix) ? "" : $"{prefix}_") + Guid.NewGuid().ToString().Remove("-");

    public static string GetEpochId(string prefix = "")
        => (string.IsNullOrEmpty(prefix) ? "" : $"{prefix}_") +
           ((long) DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMicroseconds).ToString();
    
    public static string GetEpochGuidId(string prefix = "")
        => (string.IsNullOrEmpty(prefix) ? "" : $"{prefix}_") +
           Guid.NewGuid().ToString().Split("-").First() + ((long) DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds);
    
    public static string GetId(string prefix, string extension)
        => (string.IsNullOrEmpty(prefix) ? "" : $"{prefix}_") +  Guid.NewGuid().ToString().Remove("-") + $".{extension}";
    
    public static string RandomLengthId(int numberOfCharacters = 5)
    {
        var sb = new StringBuilder(numberOfCharacters);
        for (int i = 0; i < numberOfCharacters; i++)
        {
            sb.Append(_chars[_random.Next(_chars.Length)]);
        }
        return sb.ToString();
    }

    public static string GetNumericId(int length, int? startWith = null)
    {
        var result = startWith.HasValue ? startWith.ToString() : "";
        var rand = new Random();
        for (int i = result.Length; i < length; i++)
            result += rand.Next(0, 10).ToString();

        return result;
    }
}