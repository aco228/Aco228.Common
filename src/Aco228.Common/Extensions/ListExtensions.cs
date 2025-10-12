using System.Runtime.CompilerServices;
using Aco228.Common.Models;

namespace Aco228.Common.Extensions;

public static class ListExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove<T>(this List<T> list, Func<T, bool> predicate)
    {
        foreach (var element in list.Where(predicate).ToList())
            list.Remove(element);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> GetAddRange<T>(this List<T> list, List<T> toBeInserted)
    {
        ArgumentNullException.ThrowIfNull(list);
        var response = list.ToList();
        response.AddRange(toBeInserted);
        return response;
    }
    
    public static ConcurrentList<T> ToConcurrentList<T>(this IEnumerable<T> input)
        => new ConcurrentList<T>(input.ToList());
}