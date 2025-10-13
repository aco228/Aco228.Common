namespace Aco228.Common.Infrastructure;

public class OrderedList<T> : List<T>
{
    private int _currentIndex = 0;

    public OrderedList () { }

    public OrderedList (List<T> input)
    {
        AddRange(input);
        _currentIndex = 0;
    }

    public T Take()
    {
        if (Count == 0)
            return default;

        if (_currentIndex >= Count)
            _currentIndex = 0;

        var result = this.ElementAt(_currentIndex);
        _currentIndex++;
        return result;
    }
    

    public T? TakeAndRemove()
    {
        var elem = Take();
        if (elem == null)
            return default;

        Remove(elem);
        return elem;
    }
}