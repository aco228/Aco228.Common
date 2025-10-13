using Aco228.Common.Extensions;

namespace Aco228.Common.Infrastructure;

public class ManagedList<T> : List<T>
{
    private object lockObj = new();
    private int _currentIndex = 0;

    public ManagedList () { }
    
    public ManagedList (List<T> input)
    {
        AddRange(input.Shuffle());
        _currentIndex = 0;
    }

    public ManagedList<T>  ShuffleAgain()
    {
        this.Shuffle();
        _currentIndex = 0;
        return this;
    }

    public T? TakeAndRemove()
    {
        var elem = Take();
        if (elem == null)
            return default;

        Remove(elem);
        return elem;
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
        var limit = number >= Count ? Count : number;
        for (int i = 0; i < limit; i++)
        {
            var elem = Take();
            if (elem == null)
                continue;
            
            result.Add(elem);
            if (remove)
                Remove(elem);
        }
        return result;
    }

    public void Reset()
    {
        var resufle = this.Shuffle().ToList();
        Clear();
        AddRange(resufle);
        _currentIndex = 0;
    }
    
}