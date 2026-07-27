namespace Aco228.Common.Models;

public class TimedObject<T>
{
    public T? Data { get; private set; }
    public DateTime? ValidUntil { get; private set; } = null;

    public T AddOrUpdate(T entry, DateTime? validUntil = null)
    {
        Data = entry;
        ValidUntil = validUntil ?? DateTime.UtcNow.AddMinutes(5);
        return Data;
    }

    public bool IsNotValid()
    {
        return (Data == null || ValidUntil == null || ValidUntil < DateTime.UtcNow);
    }

    public bool TryGet(out T value)
    {
        value = Get();
        return value != null;
    }

    public T? Get()
    {
        if (Data == null || ValidUntil == null || ValidUntil < DateTime.UtcNow)
            return default;

        return Data;
    }
}