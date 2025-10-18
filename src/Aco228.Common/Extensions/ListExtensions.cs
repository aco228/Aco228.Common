using System.Runtime.CompilerServices;
using Aco228.Common.Infrastructure;
using Aco228.Common.Models;

namespace Aco228.Common.Extensions;

public static class ListExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveOne<T>(this List<T> list, Func<T, bool> predicate)
    {
        foreach (var element in list.Where(predicate).ToList())
            list.Remove(element);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveOne<T>(this ConcurrentList<T> list, Func<T, bool> predicate)
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Shared.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }
    
    public static ManagedList<T> ToManagedList<T>(this List<T> input)
    {
        input.Shuffle();
        return new ManagedList<T>(input);
    }
    
    public static OrderedList<T> ToOrderedList<T>(this List<T> input)
        => new OrderedList<T>(input);
    
    public static ManagedList<T> ToManagedList<T>(this IEnumerable<T> input)
        => new ManagedList<T>(input.ToList());
    
    public static OrderedList<T> ToOrderedList<T>(this IEnumerable<T> input)
        => new OrderedList<T>(input.ToList());
    
    public static ConcurrentList<T> ToConcurrentList<T>(this IEnumerable<T> input)
        => new ConcurrentList<T>(input.ToList());

    public static List<T> AppendList<T>(this List<T> list, IEnumerable<T> append)
    {
        list.AddRange(append);
        return list;
    }
}