namespace ApplicationLayer.Common.Enums;

public sealed class ChannelPostType
{
    public string Name { get; }
    public int Value { get; }

    private ChannelPostType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly ChannelPostType Signal = new(nameof(Signal), 1);
    public static readonly ChannelPostType News = new(nameof(News), 2);

    private static readonly Dictionary<string, ChannelPostType> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(Signal), Signal },
        { nameof(News), News }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, ChannelPostType> _byValue = new()
    {
        { Signal.Value, Signal },
        { News.Value, News }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static ChannelPostType FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out ChannelPostType t) => _byValue.TryGetValue(value, out t);
}