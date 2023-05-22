public class Health
{
    public float MaxValue { get; }
    private float _value;

    public float Value
    {
        get => _value;
        set => _value = value > 0 ? value : 0;
    }

    public Health(float value)
    {
        MaxValue = value;
        Value = value;
    }

    void Restore(float value)
    {
        Value += value;
    }

    void Reduce(float value)
    {
        Value -= value;
    }
}