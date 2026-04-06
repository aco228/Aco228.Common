using System.IO.Enumeration;

namespace Aco228.Common.Extensions;

public static class HashSetExtensions
{
    public static bool ContainsSameElements<T>(this HashSet<T> source, HashSet<T> refer)
    {
        return source.SetEquals(refer);
    }
}