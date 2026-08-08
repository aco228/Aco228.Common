using System.Collections;
namespace Aco228.Common.Infrastructure;

public class OrderedList<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();
    private readonly object _lock = new();
    private int _currentIndex = 0;

    public OrderedList() { }

    public OrderedList(IEnumerable<T> input)
    {
        lock (_lock)
        {
            _items.AddRange(input);
            _currentIndex = 0;
        }
    }

    public int Count
    {
        get { lock (_lock) return _items.Count; }
    }

    public T? Take()
    {
        lock (_lock)
        {
            if (_items.Count == 0)
                return default;

            if (_currentIndex >= _items.Count)
                _currentIndex = 0;

            var result = _items[_currentIndex];
            _currentIndex++;
            return result;
        }
    }

    public T? TakeAndRemove()
    {
        lock (_lock)
        {
            if (_items.Count == 0)
                return default;

            if (_currentIndex >= _items.Count)
                _currentIndex = 0;

            var result = _items[_currentIndex];
            _items.RemoveAt(_currentIndex);
            return result;
        }
    }

    public void Add(T item)
    {
        lock (_lock) _items.Add(item);
    }

    public void AddRange(IEnumerable<T> items)
    {
        lock (_lock) _items.AddRange(items);
    }

    public void Insert(int index, T item)
    {
        lock (_lock)
        {
            _items.Insert(index, item);

            // keep the round-robin cursor sane if we inserted ahead of it
            if (index <= _currentIndex)
                _currentIndex++;
        }
    }

    public void RemoveAt(int index)
    {
        lock (_lock)
        {
            _items.RemoveAt(index);

            // keep the round-robin cursor sane if we removed ahead of it
            if (index < _currentIndex)
                _currentIndex--;
            if (_currentIndex >= _items.Count)
                _currentIndex = 0;
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            var index = _items.IndexOf(item);
            if (index < 0)
                return false;

            _items.RemoveAt(index);
            if (index < _currentIndex)
                _currentIndex--;
            if (_currentIndex >= _items.Count)
                _currentIndex = 0;

            return true;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
            _currentIndex = 0;
        }
    }

    public bool Contains(T item)
    {
        lock (_lock) return _items.Contains(item);
    }

    public T this[int index]
    {
        get { lock (_lock) return _items[index]; }
        set { lock (_lock) _items[index] = value; }
    }

    // Snapshot enumeration so iteration is safe even if another thread mutates the list mid-loop.
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;
        lock (_lock) snapshot = new List<T>(_items);
        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}