using System.Collections;

namespace Aco228.Common.Infrastructure
{
    public enum OrderedCollectionType
    {
        FIFO,
        LIFO,
    }

    public class OrderedCollection<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _items = new();
        private readonly object _lock = new();
        private readonly SemaphoreSlim _signal = new(0);
        private readonly OrderedCollectionType _type;
        private readonly bool _removeOnPop;

        public OrderedCollection(OrderedCollectionType type = OrderedCollectionType.FIFO, bool removeOnPop = true)
        {
            _type = type;
            _removeOnPop = removeOnPop;
        }

        public OrderedCollectionType Type => _type;

        public int Count
        {
            get { lock (_lock) return _items.Count; }
        }

        public bool IsEmpty => Count == 0;

        public void Add(T item)
        {
            lock (_lock)
            {
                _items.AddLast(item);
            }
            _signal.Release();
        }

        public void AddRange(IEnumerable<T> items)
        {
            var count = 0;
            lock (_lock)
            {
                foreach (var item in items)
                {
                    _items.AddLast(item);
                    count++;
                }
            }
            if (count > 0)
                _signal.Release(count);
        }

        public bool TryTake(out T item) => TryTakeInternal(out item);

        public T? TryPop()
        {
            return TryTakeInternal(out var item) ? item : default;
        }

        public bool TryPeek(out T item)
        {
            lock (_lock)
            {
                if (_items.Count == 0)
                {
                    item = default!;
                    return false;
                }

                item = _type == OrderedCollectionType.FIFO ? _items.First!.Value : _items.Last!.Value;
                return true;
            }
        }

        public bool TryTake(out T item, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var deadline = DateTime.UtcNow + timeout;

            while (true)
            {
                if (TryTakeInternal(out item))
                    return true;

                var remaining = deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    item = default!;
                    return false;
                }

                try
                {
                    if (!_signal.Wait(remaining, cancellationToken))
                    {
                        item = default!;
                        return false;
                    }
                }
                catch (OperationCanceledException)
                {
                    item = default!;
                    return false;
                }
            }
        }

        public T Take(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                if (TryTakeInternal(out var item))
                    return item;

                _signal.Wait(cancellationToken);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
            }
        }

        public T[] ToArray()
        {
            lock (_lock)
            {
                var result = new T[_items.Count];
                if (_type == OrderedCollectionType.FIFO)
                {
                    _items.CopyTo(result, 0);
                }
                else
                {
                    var i = result.Length - 1;
                    foreach (var item in _items)
                        result[i--] = item;
                }
                return result;
            }
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool TryTakeInternal(out T item)
        {
            lock (_lock)
            {
                if (_items.Count == 0)
                {
                    item = default!;
                    return false;
                }

                if (_type == OrderedCollectionType.FIFO)
                {
                    item = _items.First!.Value;
                    _items.RemoveFirst();
                    if (!_removeOnPop)
                        _items.AddLast(item);
                }
                else
                {
                    item = _items.Last!.Value;
                    _items.RemoveLast();
                    if (!_removeOnPop)
                        _items.AddFirst(item);
                }

                return true;
            }
        }
    }
}