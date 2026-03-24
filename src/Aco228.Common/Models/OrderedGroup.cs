using Aco228.Common.Extensions;
using Aco228.Common.Infrastructure;

namespace Aco228.Common.Models;

public abstract class OrderedGroup<T>
{
    public HashSet<string> Keys { get; set; } = new();
    public OrderedList<OrderedList<T>> Result { get; private set; } = new();
    private int _currentIndex = 0;

    protected List<T> GetFilteredData(IEnumerable<T> input, int? howManyToTake = null)
    {
        var filter = new List<T>();
        foreach (var entry in input)
        {
            if (howManyToTake != null && filter.Count >= howManyToTake.Value)
                break;
            
            if(!FilteredExists(entry))
                filter.Add(entry);
        }
        
        return filter;
    }

    protected bool FilteredExists(T entry)
    {
        var key = GetKeyFor(entry);
        if (Keys.Contains(key))
            return true;

        Keys.Add(key);
        return false;
    }
    
    protected abstract string GetKeyFor(T entry);

    public void Add(IEnumerable<T> input, int? howManyToTake = null)
    {
        if (!input.Any())
            return;

        var filtered = GetFilteredData(input, howManyToTake).ToOrderedList();
        if (!filtered.Any())
            return;
        
        Result.Add(filtered);
    }

    public OrderedGroup<T> SplitAndAdd(IEnumerable<T> input, int howManySplits)
    {
        var result = new List<List<T>>();
        for(int i = 0; i < howManySplits; i++)
            result.Add(new());

        var index = 0;
        for (int i = 0; i < input.Count(); i++)
        {
            result.ElementAt(index).Add(input.ElementAt(i));
            index++;
            
            if (index >= howManySplits)
                index = 0;
        }
        
        for(int i = 0; i < howManySplits; i++)
            Add(result.ElementAt(i));

        return this;
    }

    public void AddAtStart(IEnumerable<T> input, int? howManyToTake = null)
    {
        if (!input.Any())
            return;

        var filtered = GetFilteredData(input, howManyToTake).ToOrderedList();
        if (!filtered.Any())
            return;
        
        Result.Insert(0, filtered);
    }

    public bool Any() => Result.Any();

    public T? Take()
    {
        void RecalibrateIndex()
        {
            if (_currentIndex >= Result.Count)
                _currentIndex = Result.Count - 1;

            if (_currentIndex < 0)
                _currentIndex = 0;

            if (_currentIndex >= Result.Count)
                _currentIndex = Result.Count - 1;
        }
        
        if (!Any())
            return default;
        
        var list = Result.ElementAt(_currentIndex);
        if (!list.Any())
        {
            Result.RemoveAt(_currentIndex);
            RecalibrateIndex();
            return Take();
        }

        var entry = Result.ElementAt(_currentIndex).TakeAndRemove();
        if (!Result.ElementAt(_currentIndex).Any())
        {
            Result.RemoveAt(_currentIndex);
            RecalibrateIndex();
        }
        else
        {
            _currentIndex++;
            if (_currentIndex >= Result.Count)
                _currentIndex = 0;   
        }

        return entry;
    }
    
    
}