using System.Collections.Concurrent;

namespace Aco228.Common.Models;

public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dict = new();

    public bool Add(T item) => _dict.TryAdd(item, 0);
    public bool Remove(T item) => _dict.TryRemove(item, out _);
    public bool Contains(T item) => _dict.ContainsKey(item);
    public int Count => _dict.Count;
    public IEnumerable<T> Items => _dict.Keys;
}