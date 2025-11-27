namespace ApplicationLayer.Common.Enums;

public sealed class ChannelCategory
{
    public string Name { get; }
    public int Value { get; }

    private ChannelCategory(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public static readonly ChannelCategory SpotSignals = new(nameof(SpotSignals), 1);
    public static readonly ChannelCategory FuturesSignals = new(nameof(FuturesSignals), 2);
    public static readonly ChannelCategory Announcements = new(nameof(Announcements), 3);

    private static readonly Dictionary<string, ChannelCategory> _byName = new(StringComparer.OrdinalIgnoreCase)
    {
        { nameof(SpotSignals), SpotSignals },
        { nameof(FuturesSignals), FuturesSignals },
        { nameof(Announcements), Announcements }
    };

    public static bool IsValid(string name) => _byName.ContainsKey(name);

    private static readonly Dictionary<int, ChannelCategory> _byValue = new()
    {
        { SpotSignals.Value, SpotSignals },
        { FuturesSignals.Value, FuturesSignals },
        { Announcements.Value, Announcements }
    };

    public static bool IsValid(int value) => _byValue.ContainsKey(value);
    public static ChannelCategory FromValue(int value) => _byValue[value];
    public static bool TryFromValue(int value, out ChannelCategory cat) => _byValue.TryGetValue(value, out cat);
}