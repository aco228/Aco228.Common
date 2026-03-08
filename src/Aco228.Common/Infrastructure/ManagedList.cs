using Aco228.Common.Extensions;

namespace Aco228.Common.Infrastructure;

public class ManagedList<T> : List<T>
{
    private object lockObj = new();
    private int _currentIndex = 0;

    public ManagedList() { }

    public ManagedList(List<T> input)
    {
        AddRange(input.Shuffle());
        _currentIndex = 0;
    }

    public ManagedList<T> ShuffleAgain()
    {
        this.Shuffle();
        _currentIndex = 0;
        return this;
    }

    public T? TakeAndRemove()
    {
        lock (lockObj)
        {
            if (Count == 0)
                return default;

            if (_currentIndex >= Count)
                _currentIndex = 0;

            T result = this[_currentIndex];
            RemoveAt(_currentIndex);
            // don't increment — next element shifted into current index
            return result;
        }
    }

    public T? TakeRandom(Func<T, bool>? filterExpression = null)
    {
        lock (lockObj)
        {
            if (Count == 0)
                return default;

            var collection = this;
            if (filterExpression != null)
                collection = this.Where(filterExpression).ToManagedList();

            return collection.Take();
        }
    }

    public T? Take()
    {
        lock (lockObj)
        {
            if (Count == 0)
                return default;

            if (_currentIndex >= Count)
                _currentIndex = 0;

            T? result = this.ElementAt(_currentIndex);
            _currentIndex++;

            return result;
        }
    }

    public ManagedList<T> TakeNum(int number, bool remove = false)
    {
        var result = new ManagedList<T>();
        var limit = Math.Min(number, Count);
        for (int i = 0; i < limit; i++)
        {
            var elem = Take();
            if (elem == null)
                continue;

            result.Add(elem);
            if (remove)
            {
                var idx = IndexOf(elem);
                if (idx >= 0)
                {
                    RemoveAt(idx);
                    if (idx < _currentIndex)
                        _currentIndex--; // compensate for shifted index
                }
            }
        }
        return result;
    }

    public void Reset()
    {
        var reshuffled = this.Shuffle().ToList();
        Clear();
        AddRange(reshuffled);
        _currentIndex = 0;
    }
}