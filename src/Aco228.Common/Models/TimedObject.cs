namespace Aco228.Common.Models;

public class TimedObject<T>
{
    private bool _hasValue;
    public T? Data { get; private set; }
    public DateTime? ValidUntil { get; private set; }

    public T AddOrUpdate(T entry, DateTime? validUntil = null)
    {
        Data = entry;
        _hasValue = true;
        ValidUntil = validUntil ?? DateTime.Now.AddMinutes(5);
        return entry;
    }

    public bool IsNotValid()
        => !_hasValue || ValidUntil == null || ValidUntil < DateTime.Now;

    public bool TryGet(out T value)
    {
        if (IsNotValid())
        {
            value = default!;
            return false;
        }
        value = Data!;
        return true;
    }

    public T? Get() => IsNotValid() ? default : Data;
}