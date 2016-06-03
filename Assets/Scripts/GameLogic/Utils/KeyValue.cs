public class KeyValue<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public KeyValue()
    {
    }

    public KeyValue(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}