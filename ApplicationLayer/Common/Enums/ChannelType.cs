namespace ApplicationLayer.Common.Enums;

public sealed class ChannelType
{
    public string Name { get; }
    public int Value { get; }

    private ChannelType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly ChannelType News = new(nameof(News), 1);
    public static readonly ChannelType Signal = new(nameof(Signal), 2);

    private static readonly Dictionary<string, ChannelType> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(News), News },
        { nameof(Signal), Signal }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, ChannelType> _byValue = new()
    {
        { News.Value, News },
        { Signal.Value, Signal }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static ChannelType FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out ChannelType type) => _byValue.TryGetValue(value, out type);
}
