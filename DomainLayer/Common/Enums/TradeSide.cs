namespace DomainLayer.Common.Enums;

public sealed class TradeSide
{
    public string Name { get; }
    public int Value { get; }

    private TradeSide(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly TradeSide Long = new(nameof(Long), 1);
    public static readonly TradeSide Short = new(nameof(Short), 2);

    private static readonly Dictionary<string, TradeSide> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Long), Long },
        { nameof(Short), Short }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, TradeSide> _byValue = new()
    {
        { Long.Value, Long },
        { Short.Value, Short }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static TradeSide FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out TradeSide t) => _byValue.TryGetValue(value, out t);
    
    // Implicit conversion to int for easy DB storage if needed
    public static implicit operator int(TradeSide t) => t.Value;
}
