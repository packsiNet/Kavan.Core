namespace DomainLayer.Common.Enums;

public sealed class TradeStatus
{
    public string Name { get; }
    public int Value { get; }

    private TradeStatus(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly TradeStatus Open = new(nameof(Open), 1);
    public static readonly TradeStatus Closed = new(nameof(Closed), 2);
    public static readonly TradeStatus Cancelled = new(nameof(Cancelled), 3);
    public static readonly TradeStatus PartiallyClosed = new(nameof(PartiallyClosed), 4);

    private static readonly Dictionary<string, TradeStatus> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Open), Open },
        { nameof(Closed), Closed },
        { nameof(Cancelled), Cancelled },
        { nameof(PartiallyClosed), PartiallyClosed }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, TradeStatus> _byValue = new()
    {
        { Open.Value, Open },
        { Closed.Value, Closed },
        { Cancelled.Value, Cancelled },
        { PartiallyClosed.Value, PartiallyClosed }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static TradeStatus FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out TradeStatus t) => _byValue.TryGetValue(value, out t);

    public static implicit operator int(TradeStatus t) => t.Value;
}
