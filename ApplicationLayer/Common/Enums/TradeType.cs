namespace ApplicationLayer.Common.Enums;

public sealed class TradeType
{
    public string Name { get; }
    public int Value { get; }

    private TradeType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly TradeType Spot = new(nameof(Spot), 1);
    public static readonly TradeType Futures = new(nameof(Futures), 2);

    private static readonly Dictionary<string, TradeType> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Spot), Spot },
        { nameof(Futures), Futures }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, TradeType> _byValue = new()
    {
        { Spot.Value, Spot },
        { Futures.Value, Futures }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static TradeType FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out TradeType t) => _byValue.TryGetValue(value, out t);
}