namespace ApplicationLayer.Common.Enums;

public sealed class IdeaTrend
{
    public string Name { get; }
    public int Value { get; }

    private IdeaTrend(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly IdeaTrend Bullish = new(nameof(Bullish), 1);
    public static readonly IdeaTrend Bearish = new(nameof(Bearish), 2);
    public static readonly IdeaTrend Range = new(nameof(Range), 3);

    private static readonly Dictionary<string, IdeaTrend> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Bullish), Bullish },
        { nameof(Bearish), Bearish },
        { nameof(Range), Range }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);
}